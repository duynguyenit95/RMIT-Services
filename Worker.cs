using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Context;
using TaskScheduler.Entity;
using TaskScheduler.Interface;
using TaskScheduler.WecomNotify;

namespace TaskScheduler
{
    public class Worker : BackgroundService
    {
        readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Dictionary<string, ITaskFunc> _taskFuncs;
        private readonly int _maxTaskRunParallel = 5;
        public Worker(
            ILogger<Worker> logger,
            IServiceProvider serviceProvider,
            IHttpClientFactory httpClientFactory,
            IEnumerable<ITaskFunc> taskFuncs)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _httpClientFactory = httpClientFactory;
            _taskFuncs = taskFuncs.ToDictionary(x => x.Name, x => x);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndSendAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task CheckAndSendAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RPAGateContext>();
            var httpClient = _httpClientFactory.CreateClient();

            var now = DateTime.Now;
            var tasks = await db.Tasks
                .Include(x => x.Scheduler)
                .Where(x => x.IsEnabled && (x.NextRunAt == null || x.NextRunAt <= now))
                .ToListAsync(stoppingToken);

            var semaphore = new SemaphoreSlim(_maxTaskRunParallel);
            var taskList = new List<Task>();

            foreach (var task in tasks)
            {
                var localTask = task;

                taskList.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync(stoppingToken);
                    try
                    {
                        switch (localTask.Type)
                        {
                            case "API":
                                await CallApi(httpClient, localTask);
                                break;
                            case "Func":
                                await CallFunc(localTask);
                                break;
                            case "WecomMessage":
                                await WecomRPAGate.SendMessage(localTask.WecomRobotId, localTask.Message, localTask.Tag);
                                break;
                        }

                        using var innerScope = _serviceProvider.CreateScope();
                        var dbInner = innerScope.ServiceProvider.GetRequiredService<RPAGateContext>();

                        var dbTask = await dbInner.Tasks.Include(x => x.Scheduler).FirstOrDefaultAsync(x => x.Id == localTask.Id);
                        if (dbTask != null)
                        {
                            dbTask.LastRunAt = now;
                            dbTask.NextRunAt = UpdateNextRunTime(dbTask, now);
                            await dbInner.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error in executing task ID={localTask.Id}");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
                await Task.WhenAll(taskList);
            }

        }
        DateTime UpdateNextRunTime(RpaTask task, DateTime now)
        {

            if (task.Scheduler.IntervalMinutes.HasValue)
                return now.AddMinutes(task.Scheduler.IntervalMinutes.Value);

            if (task.Scheduler.IntervalHours.HasValue)
                return now.AddHours(task.Scheduler.IntervalHours.Value);

            if (task.Scheduler.Time.HasValue)
            {
                var next = new DateTime(now.Year, now.Month, now.Day, task.Scheduler.Time.Value.Hours, task.Scheduler.Time.Value.Minutes, 0);

                // Nếu đã trễ, dịch sang ngày tiếp theo
                if (next <= now)
                    next = next.AddDays(1);

                // Nếu có WeekDays hoặc MonthDays thì phải tìm ngày hợp lệ tiếp theo
                while (true)
                {
                    bool validWeek = true, validMonth = true;

                    if (!string.IsNullOrEmpty(task.Scheduler.WeekDays))
                    {
                        var allowedWeekDays = task.Scheduler.WeekDays.Split(',')
                                            .Select(int.Parse).ToList();
                        int dow = (int)next.DayOfWeek;
                        if (!allowedWeekDays.Contains(dow))
                            validWeek = false;
                    }

                    if (!string.IsNullOrEmpty(task.Scheduler.MonthDays))
                    {
                        var allowedMonthDays = task.Scheduler.MonthDays.Split(',').Select(int.Parse).ToList();
                        int dom = next.Day;
                        bool isLastDay = dom == DateTime.DaysInMonth(next.Year, next.Month);

                        if (!allowedMonthDays.Contains(dom) && !(allowedMonthDays.Contains(31) && isLastDay))
                            validMonth = false;
                    }

                    if (validWeek && validMonth)
                        break;

                    next = next.AddDays(1);
                }

                return next;
            }

            throw new InvalidOperationException($"Job {task.Scheduler.Name} không có IntervalMinutes, IntervalHours hoặc Time!");
        }
                
        private async Task CallApi(HttpClient httpClient, RpaTask task)
        {
            try
            {
                var response = await httpClient.GetAsync(task.RequestUrl);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notify: {ex.Message}");
            }
        }
        private async Task CallFunc(RpaTask task)
        {
            await ExecuteFuncAsync(task.FuncName, task.WecomRobotId);
        }


        private async Task ExecuteFuncAsync(string funcName, string? wecomRobotId)
        {
            if (_taskFuncs.TryGetValue(funcName, out var func))
            {
                try
                {
                    await func.Execute(wecomRobotId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error executing function {funcName}");
                    if (!string.IsNullOrEmpty(wecomRobotId))
                    {
                        await WecomRPAGate.SendMessage(wecomRobotId, $"❌ Error executing {funcName}: {ex.Message}");
                    }
                }
            }
            else
            {
                _logger.LogWarning($"Function {funcName} not found");
                if (!string.IsNullOrEmpty(wecomRobotId))
                {
                    await WecomRPAGate.SendMessage(wecomRobotId, $"⚠️ Function `{funcName}` not found");
                }
            }
        }
    }
}
