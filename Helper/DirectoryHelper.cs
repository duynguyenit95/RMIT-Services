using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.Helper
{
    public static class DirectoryHelper
    {
        public static void CheckDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }
        }
        public static string FindDirectory(string parentPath, string keyword)
        {
            var dirs = Directory.GetDirectories(parentPath);
            var targetDir = dirs.FirstOrDefault(d => d.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            if (targetDir == null)
                throw new Exception($"Directory not found: {targetDir}");

            return targetDir;
        }

        public static string FindFile(string dirPath, string keyword, string extension)
        {
            var files = Directory.GetFiles(dirPath);
            var targetFile = files.FirstOrDefault(f =>
                f.Contains(keyword, StringComparison.OrdinalIgnoreCase) &&
                f.EndsWith(extension, StringComparison.OrdinalIgnoreCase));

            if (targetFile == null)
                throw new Exception($"File not found: {keyword} with extension {extension} in {dirPath}");
            return targetFile;
        }
        public static string CopyFile(string target_file, string destinationPath, string newName)
        {
            string destinationFilePath = Path.Combine(destinationPath, newName);
            if (File.Exists(destinationFilePath))
                File.Delete(destinationFilePath);
            File.Copy(target_file, destinationFilePath);

            return destinationFilePath;
        }
    }
}
