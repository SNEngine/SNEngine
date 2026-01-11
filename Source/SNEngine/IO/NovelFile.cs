using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using SNEngine.Debugging;
using UnityEngine;
using UnityEngine.Networking;

namespace SNEngine.IO
{
    public static class NovelFile
    {
        public static string PersistentDataPath => Application.persistentDataPath;
        public static string StreamingAssetsPath => Application.streamingAssetsPath;
        public static string DataPath => Application.dataPath;
        public static string TemporaryCachePath => Application.temporaryCachePath;

        static async UniTask<T> WithStream<T>(string path, FileMode mode, FileAccess access, FileShare share, Func<Stream, UniTask<T>> action)
        {
            using var stream = new FileStream(path, mode, access, FileShare.ReadWrite, 4096, true);
            return await action(stream);
        }

        static UniTask WithStream(string path, FileMode mode, FileAccess access, FileShare share, Func<Stream, UniTask> action) =>
            WithStream<object>(path, mode, access, share, async s => { await action(s); return null; });

        private static bool IsStreamingAssetsPathRestricted(string path)
        {
            if (path.StartsWith("jar") || path.StartsWith("http"))
            {
                return true;
            }

            return false;
        }

        static async UniTask<string> ReadStreamingAssetsTextAsync(string path, Encoding encoding = null)
        {
            using var request = UnityWebRequest.Get(path);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new IOException($"Failed to load file from StreamingAssets: {path}. Error: {request.error}");
            }
            return encoding is not null ? encoding.GetString(request.downloadHandler.data) : request.downloadHandler.text;
        }

        static async UniTask<byte[]> ReadStreamingAssetsBytesAsync(string path)
        {
            using var request = UnityWebRequest.Get(path);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                NovelGameDebug.LogError($"[NovelFile] Failed to read binary file at {path}: {request.error}");
                return null;
            }

            return request.downloadHandler.data;
        }

        public static UniTask<string> ReadAllTextAsync(string path, Encoding encoding = null)
        {
            if (IsStreamingAssetsPathRestricted(path))
            {
                return ReadStreamingAssetsTextAsync(path, encoding);
            }

            return WithStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, async s =>
            {
                encoding ??= Encoding.UTF8;
                using var reader = new StreamReader(s, encoding);
                return await reader.ReadToEndAsync();
            });
        }

        public static string ReadAllText(string path, Encoding encoding = null)
        {
            if (IsStreamingAssetsPathRestricted(path))
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    throw new NotSupportedException("Synchronous IO is not supported on WebGL for StreamingAssets. Use ReadAllTextAsync instead.");
                }

                NovelGameDebug.LogWarning("Blocking call to ReadAllText on restricted path. Use async version.");
                return ReadAllTextAsync(path, encoding).GetAwaiter().GetResult();
            }

            encoding ??= Encoding.UTF8;
            return File.ReadAllText(path, encoding);
        }

        public static UniTask WriteAllTextAsync(string path, string contents, Encoding encoding = null) =>
            WithStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, async s =>
            {
                encoding ??= Encoding.UTF8;
                using var writer = new StreamWriter(s, encoding);
                await writer.WriteAsync(contents);
            });

        public static void WriteAllText(string path, string contents, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            File.WriteAllText(path, contents, encoding);
        }

        public static UniTask<byte[]> ReadAllBytesAsync(string path)
        {
            if (IsStreamingAssetsPathRestricted(path))
            {
                return ReadStreamingAssetsBytesAsync(path);
            }

            return WithStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, async s =>
            {
                var buffer = new byte[s.Length];
                await s.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            });
        }

        public static byte[] ReadAllBytes(string path)
        {
            if (IsStreamingAssetsPathRestricted(path))
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    throw new NotSupportedException("Synchronous IO is not supported on WebGL for StreamingAssets. Use ReadAllBytesAsync instead.");
                }

                NovelGameDebug.LogWarning("Blocking call to ReadAllBytes on restricted path. Use async version.");
                return ReadAllBytesAsync(path).GetAwaiter().GetResult();
            }
            return File.ReadAllBytes(path);
        }

        public static UniTask WriteAllBytesAsync(string path, byte[] bytes) =>
            WithStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, s => s.WriteAsync(bytes, 0, bytes.Length).AsUniTask());

        public static void WriteAllBytes(string path, byte[] bytes) => File.WriteAllBytes(path, bytes);

        public static UniTask AppendAllTextAsync(string path, string contents, Encoding encoding = null) =>
            WithStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, async s =>
            {
                encoding ??= Encoding.UTF8;
                using var writer = new StreamWriter(s, encoding);
                await writer.WriteAsync(contents);
            });

        public static void AppendAllText(string path, string contents, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            File.AppendAllText(path, contents, encoding);
        }

        public static UniTask AppendAllBytesAsync(string path, byte[] bytes) =>
            WithStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, s => s.WriteAsync(bytes, 0, bytes.Length).AsUniTask());

        static async UniTask<bool> ExistsStreamingAssetsAsync(string path)
        {
            using var request = UnityWebRequest.Get(path);
            await request.SendWebRequest();
            return request.result == UnityWebRequest.Result.Success;
        }

        public static bool Exists(string path)
        {
            if (IsStreamingAssetsPathRestricted(path))
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    throw new NotSupportedException("Synchronous IO is not supported on WebGL for restricted paths. Use an asynchronous method for existence checks.");
                }

                NovelGameDebug.LogWarning("Blocking call to Exists on restricted path. Use async version.");
                return ExistsStreamingAssetsAsync(path).GetAwaiter().GetResult();
            }

            return File.Exists(path);
        }

        public static void Delete(string path) => File.Delete(path);

        public static void Move(string sourceFileName, string destFileName, bool overwrite = false)
        {
            if (overwrite && File.Exists(destFileName)) File.Delete(destFileName);
            File.Move(sourceFileName, destFileName);
        }

        public static UniTask CopyAsync(string sourceFileName, string destFileName, bool overwrite = false) =>
            UniTask.RunOnThreadPool(() =>
            {
                if (overwrite && File.Exists(destFileName)) File.Delete(destFileName);
                File.Copy(sourceFileName, destFileName, overwrite);
            });

        public static void Copy(string sourceFileName, string destFileName, bool overwrite = false) =>
            File.Copy(sourceFileName, destFileName, overwrite);

        public static DateTime GetCreationTime(string path) => File.GetCreationTime(path);
        public static DateTime GetCreationTimeUtc(string path) => File.GetCreationTimeUtc(path);
        public static void SetCreationTime(string path, DateTime creationTime) => File.SetCreationTime(path, creationTime);
        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc) => File.SetCreationTimeUtc(path, creationTimeUtc);

        public static DateTime GetLastAccessTime(string path) => File.GetLastAccessTime(path);
        public static DateTime GetLastAccessTimeUtc(string path) => File.GetLastAccessTimeUtc(path);
        public static void SetLastAccessTime(string path, DateTime lastAccessTime) => File.SetLastAccessTime(path, lastAccessTime);
        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) => File.SetLastAccessTimeUtc(path, lastAccessTimeUtc);

        public static DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);
        public static DateTime GetLastWriteTimeUtc(string path) => File.GetLastWriteTimeUtc(path);
        public static void SetLastWriteTime(string path, DateTime lastWriteTime) => File.SetLastWriteTime(path, lastWriteTime);
        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) => File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);

        public static FileStream Open(string path, FileMode mode) => File.Open(path, mode);
        public static FileStream Open(string path, FileMode mode, FileAccess access) => File.Open(path, mode, access);
        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share) => File.Open(path, mode, access, share);

        public static FileStream OpenRead(string path) => File.OpenRead(path);
        public static FileStream OpenWrite(string path) => File.OpenWrite(path);

        public static StreamReader OpenText(string path) => File.OpenText(path);

        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName) =>
            File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);

        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors) =>
            File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);

        public static string GetAbsolutePath(string path)
        {
            if (IsStreamingAssetsPathRestricted(path))
            {
                return path;
            }

            if (Path.IsPathRooted(path))
            {
                return path;
            }

            try
            {
                return Path.GetFullPath(path);
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"Failed to get full path for {path}: {ex.Message}");
                return path;
            }
        }
    }
}