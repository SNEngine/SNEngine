using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SNEngine.IO
{
    public static class NovelDirectory
    {
        public static string PersistentDataPath => Application.persistentDataPath;
        public static string StreamingAssetsPath => Application.streamingAssetsPath;
        public static string DataPath => Application.dataPath;
        public static string TemporaryCachePath => Application.temporaryCachePath;

        public static void Create(string path) => Directory.CreateDirectory(path);
        public static bool Exists(string path) => Directory.Exists(path);
        public static void Delete(string path, bool recursive = false) => Directory.Delete(path, recursive);
        public static void Move(string sourceDirName, string destDirName) => Directory.Move(sourceDirName, destDirName);
        public static void Copy(string sourceDirName, string destDirName, bool recursive = false)
        {
            if (!Directory.Exists(sourceDirName))
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDirName}");
            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();
            Directory.CreateDirectory(destDirName);
            foreach (var file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destDirName, file.Name);
                file.CopyTo(targetFilePath, true);
            }
            if (recursive)
            {
                foreach (var subDir in dirs)
                {
                    string newDest = Path.Combine(destDirName, subDir.Name);
                    Copy(subDir.FullName, newDest, true);
                }
            }
        }
        public static string[] GetFiles(string path) => Directory.GetFiles(path);
        public static string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);
        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption) => Directory.GetFiles(path, searchPattern, searchOption);
        public static string[] GetDirectories(string path) => Directory.GetDirectories(path);
        public static string[] GetDirectories(string path, string searchPattern) => Directory.GetDirectories(path, searchPattern);
        public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) => Directory.GetDirectories(path, searchPattern, searchOption);

        public static UniTask CreateAsync(string path) => UniTask.RunOnThreadPool(() => Directory.CreateDirectory(path));
        public static UniTask DeleteAsync(string path, bool recursive = false) => UniTask.RunOnThreadPool(() => Directory.Delete(path, recursive));
        public static UniTask<string[]> GetFilesAsync(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
            UniTask.RunOnThreadPool(() => Directory.GetFiles(path, searchPattern, searchOption));
        public static UniTask<string[]> GetDirectoriesAsync(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
            UniTask.RunOnThreadPool(() => Directory.GetDirectories(path, searchPattern, searchOption));
    }
}
