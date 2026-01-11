using Cysharp.Threading.Tasks;
using SNEngine.SaveSystem.Models;
using System.Collections.Generic;
using UnityEngine;

namespace SNEngine.SaveSystem
{
    public interface ISaveLoadProvider
    {
        UniTask SaveAsync(string saveName, SaveData data, Texture2D previewTexture);
        UniTask<PreloadSave> LoadPreloadSaveAsync(string saveName);
        UniTask<SaveData> LoadSaveAsync(string saveName);
        UniTask<IEnumerable<string>> GetAllAvailableSavesAsync();
        UniTask<bool> DeleteSaveAsync(string saveName);
    }
}