using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SNEngine.Debugging;
using SNEngine.IO;
using SNEngine.SaveSystem.Models;
using SNEngine.SnapshotSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Snapshot Service")]
    public class SnapshotService : ServiceBase, IService, IDisposable
    {
        private readonly Stack<SaveData> _historyStack = new();
        private ISnapshotProvider _provider;
        private bool _isWriting;

#if UNITY_EDITOR
        private CancellationTokenSource _debugThrottleCts;
#endif

        public override void Initialize()
        {
            _historyStack.Clear();
        }

        public void BindToSlot(string saveName)
        {
            if (_provider is IDisposable d) d.Dispose();

#if UNITY_WEBGL && !UNITY_EDITOR
            _provider = new PlayerPrefsSnapshotProvider(saveName);
#else
            _provider = new FileSnapshotProvider(saveName);
#endif
            SyncStackToDisk().Forget();
        }

        public void PushSnapshot(SaveData data)
        {
            if (data == null || string.IsNullOrEmpty(data.CurrentNode)) return;

            if (_historyStack.Count > 0)
            {
                if (_historyStack.Peek().Equals(data))
                {
                    return;
                }
            }

            _historyStack.Push(data);

#if UNITY_EDITOR
            ScheduleDebugWrite();
#endif

            if (_provider != null)
            {
                SaveToProviderAsync(data).Forget();
            }
        }

        public async UniTask<SaveData> PopSnapshotAsync()
        {
            if (_historyStack.Count > 0)
            {
                var data = _historyStack.Pop();
                if (_provider != null) _provider.PopLastAsync().Forget();

#if UNITY_EDITOR
                ScheduleDebugWrite();
#endif
                return data;
            }

            if (_provider != null)
            {
                byte[] raw = await _provider.PopLastAsync();
                return raw != null ? Deserialize(raw) : null;
            }

            return null;
        }

        private async UniTask SyncStackToDisk()
        {
            if (_provider == null || _historyStack.Count == 0) return;

            var items = _historyStack.ToArray();
            Array.Reverse(items);

            foreach (var item in items)
            {
                await SaveToProviderAsync(item);
            }
        }

        private async UniTask SaveToProviderAsync(SaveData data)
        {
            while (_isWriting) await UniTask.Yield();
            _isWriting = true;
            try
            {
                await _provider.AppendAsync(Serialize(data));
            }
            finally
            {
                _isWriting = false;
            }
        }

#if UNITY_EDITOR
        private void ScheduleDebugWrite()
        {
            _debugThrottleCts?.Cancel();
            _debugThrottleCts = new CancellationTokenSource();
            WriteDebugWithDelay(_debugThrottleCts.Token).Forget();
        }

        private async UniTaskVoid WriteDebugWithDelay(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: token);

                var historyList = _historyStack.ToList();
                string debugJson = JsonConvert.SerializeObject(historyList, Formatting.Indented);
                string debugPath = Path.Combine(Application.persistentDataPath, "last_history_stack_debug.json");

                await NovelFile.WriteAllTextAsync(debugPath, debugJson);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                NovelGameDebug.LogWarning($"[SnapshotService] Debug write failed: {ex.Message}");
            }
        }
#endif

        private byte[] Serialize(SaveData data)
        {
            string json = JsonConvert.SerializeObject(data);
            byte[] payload = Encoding.UTF8.GetBytes(json);
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            writer.Write(DeriveSmartGuid(data.CurrentNode).ToByteArray());
            writer.Write(payload.Length);
            writer.Write(payload);
            return ms.ToArray();
        }

        private SaveData Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);
            ms.Position = 16;
            int len = reader.ReadInt32();
            return JsonConvert.DeserializeObject<SaveData>(Encoding.UTF8.GetString(reader.ReadBytes(len)));
        }

        private Guid DeriveSmartGuid(string nodeGuidStr)
        {
            if (!Guid.TryParse(nodeGuidStr, out Guid nodeGuid)) return Guid.NewGuid();
            byte[] b = nodeGuid.ToByteArray();
            using var md5 = MD5.Create();
            byte[] h = md5.ComputeHash(b);
            h[^1] = b[^1];
            return new Guid(h);
        }

        public void ClearHistory()
        {
            _historyStack.Clear();
            _provider?.ClearAsync().Forget();
#if UNITY_EDITOR
            ScheduleDebugWrite();
#endif
        }

        public void Dispose()
        {
            if (_provider is IDisposable d) d.Dispose();
            _provider = null;
            _historyStack.Clear();
#if UNITY_EDITOR
            _debugThrottleCts?.Cancel();
#endif
        }
    }
}