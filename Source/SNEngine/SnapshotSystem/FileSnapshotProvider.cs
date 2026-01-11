using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SNEngine.SnapshotSystem
{
    public class FileSnapshotProvider : ISnapshotProvider, IDisposable
    {
        private readonly string _path;
        private FileStream _fs;

        public FileSnapshotProvider(string saveName)
        {
            string folder = Path.Combine(Application.persistentDataPath, "saves", saveName);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            _path = Path.Combine(folder, "history.snss");
            _fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _fs.Seek(0, SeekOrigin.End);
        }

        public async UniTask AppendAsync(byte[] data)
        {
            await _fs.WriteAsync(data, 0, data.Length);
            await _fs.FlushAsync();
        }

        public async UniTask<byte[]> PopLastAsync()
        {
            if (_fs.Length == 0) return null;

            return await UniTask.RunOnThreadPool(() =>
            {
                long lastPos = FindLastBlockOffset();
                if (lastPos < 0) return null;

                _fs.Position = lastPos;
                int totalToRead = (int)(_fs.Length - lastPos);
                byte[] buffer = new byte[totalToRead];
                _fs.Read(buffer, 0, totalToRead);

                _fs.SetLength(lastPos);
                _fs.Seek(0, SeekOrigin.End);
                return buffer;
            });
        }

        private long FindLastBlockOffset()
        {
            long currentPos = 0;
            long lastPos = -1;
            _fs.Position = 0;
            using (var reader = new BinaryReader(_fs, System.Text.Encoding.UTF8, true))
            {
                while (currentPos < _fs.Length)
                {
                    lastPos = currentPos;
                    _fs.Position = currentPos + 16;
                    int len = reader.ReadInt32();
                    currentPos = _fs.Position + len;
                }
            }
            return lastPos;
        }

        public UniTask ClearAsync()
        {
            _fs.SetLength(0);
            return UniTask.CompletedTask;
        }

        public void Dispose() => _fs?.Dispose();
    }
}