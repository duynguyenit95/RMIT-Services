using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Security.Cryptography;
using TaskScheduler;
using TaskScheduler.Context;
using TaskScheduler.FileDelivery;
using TaskScheduler.Helper;
using TaskScheduler.Interface;

namespace TaskScheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + $"/Log/log-{DateTime.Today.ToString("yyyyMMdd")}.txt", Serilog.Events.LogEventLevel.Warning).CreateLogger();

            try
            {
                Log.Information("Starting up...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .UseWindowsService()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var enviroment = hostingContext.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
                          .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                var config = context.Configuration;

                services.AddDbContext<RPAGateContext>(options =>
                    options.UseSqlServer(config.GetConnectionString("RPAConnection")));

                services.AddHttpClient();

                services.AddHostedService<Worker>();

                services.Scan(scan => scan.FromAssembliesOf(typeof(ITaskFunc))
                                          .AddClasses(c => c.AssignableTo<ITaskFunc>())
                                          .AsImplementedInterfaces()
                                          .WithSingletonLifetime());

                services.AddSingleton<FileMover>();
                services.AddSingleton<ConfigService>();
            });
    }

}