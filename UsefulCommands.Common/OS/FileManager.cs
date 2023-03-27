using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace UsefulCommands.Common
{
    public class FileManager
    {
        public static void DeleteDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(directoryPath));
            }

            DeleteDirectory(new DirectoryInfo(directoryPath));
        }

        public static void DeleteDirectory(DirectoryInfo directoryInfo)
        {
            if (directoryInfo.Exists)
            {
                CleanDirectory(directoryInfo);
                directoryInfo.Delete();
            }
        }

        public static void CleanDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(directoryPath));
            }

            CleanDirectory(new DirectoryInfo(directoryPath));
        }

        public static void CleanDirectory(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            if (directoryInfo.Exists)
            {
                directoryInfo.Attributes = FileAttributes.Normal;

                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    DeleteFile(fileInfo);
                }

                foreach (var subdirectory in directoryInfo.GetDirectories())
                {
                    DeleteDirectory(subdirectory);
                }
            }
        }

        static void AttemptDeletionWithRetry(FileSystemInfo info)
        {
            const int MaxDeletionAttempts = 5;
            int attemptsMade = 0;

            while (attemptsMade < MaxDeletionAttempts)
            {
                try
                {
                    info.Delete();
                    return;
                }
                catch (FileNotFoundException)
                {
                    return;
                }
                catch (IOException)
                {
                    attemptsMade++;
                }
                catch (UnauthorizedAccessException)
                {
                    attemptsMade++;
                }

                if (attemptsMade < MaxDeletionAttempts)
                {
                    Thread.Sleep(300);
                }
            }
        }

        public static void DeleteFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(file));
            }

            DeleteFile(new FileInfo(file));
        }

        public static void DeleteFile(FileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (fileInfo.Exists)
            {
                fileInfo.IsReadOnly = false;
                AttemptDeletionWithRetry(fileInfo);
            }
        }

        public static void CopyDirectory(string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(targetPath);
            foreach (var sourceFilePath in Directory.GetFiles(sourcePath))
            {
                File.Copy(sourceFilePath, Path.Combine(targetPath, Path.GetFileName(sourceFilePath)), true);
            }
            foreach (var directory in Directory.GetDirectories(sourcePath))
            {
                CopyDirectory(directory, Path.Combine(targetPath, Path.GetFileName(directory)));
            }
        }
    }
}
