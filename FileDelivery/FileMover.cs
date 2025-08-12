using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Helper;
using TaskScheduler.Interface;
using TaskScheduler.WecomNotify;

namespace TaskScheduler.FileDelivery
{
    public class FileMover
    {
        private readonly ConfigService _configServices;
        private readonly string _wecomRobotId = "e12e41bd-eab0-4b58-9736-056b651cbaeb";
        public FileMover(ConfigService configServices, IHttpClientFactory httpClientFactory)
        {
            _configServices = configServices;
        }
        public async Task MoveFile(string configGroup, string configKey, string folderPath, string destinationPath, string[] terracedPath, string fileName, string extension, string? wecomRobotId)
        {
            string subject = $"{configGroup} - {configKey}";
            wecomRobotId ??= _wecomRobotId;
            try
            {
                DirectoryHelper.CheckDirectoryExists(folderPath);
                string currentPath = folderPath;
                foreach (var path in terracedPath)
                {
                    currentPath = DirectoryHelper.FindDirectory(currentPath, path);
                }
                var target_file = DirectoryHelper.FindFile(currentPath, fileName, extension);
                var resultFile = DirectoryHelper.CopyFile(target_file, destinationPath, Path.GetFileName(target_file));
                await _configServices.UpdateSystemConfig(configGroup, configKey, resultFile);
                await SendSucess(subject, target_file, resultFile, wecomRobotId);
            }
            catch (Exception ex)
            {
                await SendError(subject, ex.Message, wecomRobotId);
            }
        }
        public static async Task SendError(string subject, string message, string wecomRobotId)
        {
            await WecomRPAGate.SendMessage(wecomRobotId, $"{subject}\n❌Error in FileMover: {message}");
        }
        public static async Task SendSucess(string subject, string targetFile, string destinationFile, string wecomRobotId)
        {
            await WecomRPAGate.SendMessage(wecomRobotId, $"{subject}\n✅Successfully\nTargetFile: {targetFile}\nDestination File: {destinationFile}");
        }
    }
}
