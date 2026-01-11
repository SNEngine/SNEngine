using UnityEngine;
using SNEngine.SaveSystem;
using System;

namespace SNEngine.SaveSystem.Models
{
    [Serializable]
    public class PreloadSave : IDisposable
    {
        public Texture2D PreviewTexture { get; set; }
        public SaveData SaveData { get; set; }
        public string SaveName { get; set; }

        public void Dispose()
        {
            if (PreviewTexture != null)
            {
                UnityEngine.Object.Destroy(PreviewTexture);
            }
            SaveData = null;
            SaveName = null;
        }
    }
}