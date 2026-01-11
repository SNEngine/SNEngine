using Cysharp.Threading.Tasks;
using SNEngine.Debugging;
using SNEngine.IO;
using System;
using System.IO;
using UnityEngine;

namespace SNEngine.Editor.Language.Workers
{
    [CreateAssetMenu(menuName = "SNEngine/Editor/Language/Workers/Flags Language Worker")]
    public class FlagsLanguageWorkerEditor : LanguageEditorWorker
    {
        public static string PathSave { get; set; }
        private const string PLACEHOLDER_FLAG = "placeholder_flag.png";
        private const string FLAGS_FOLDER = "Assets/SNEngine/Source/SNEngine/Editor/Sprites/Flags";
        private const string OUTPUT_FLAG_NAME = "flag.png";

        public static string FlagToUse { get; set; }

        public override async UniTask<LanguageWorkerResult> Work()
        {
            LanguageWorkerResult result = new();

            if (string.IsNullOrEmpty(PathSave))
            {
                string error = $"[{nameof(FlagsLanguageWorkerEditor)}] PathSave not set";
                NovelGameDebug.LogError(error);
                result.Message = error;
                result.State = LanguageWorkerState.Error;
                return result;
            }

            string fullPath = Path.Combine(PathSave, OUTPUT_FLAG_NAME);
            string directory = Path.GetDirectoryName(fullPath);

            if (!NovelDirectory.Exists(directory))
            {
                await NovelDirectory.CreateAsync(directory);
            }

            string flagFileName = string.IsNullOrEmpty(FlagToUse) ? PLACEHOLDER_FLAG : FlagToUse;
            string flagPath = Path.Combine(FLAGS_FOLDER, flagFileName);

            if (!File.Exists(flagPath))
            {
                string error = $"[{nameof(FlagsLanguageWorkerEditor)}] Flag file not found: {flagPath}";
                NovelGameDebug.LogError(error);
                result.Message = error;
                result.State = LanguageWorkerState.Error;
                return result;
            }

            try
            {
                byte[] flagBytes = await NovelFile.ReadAllBytesAsync(flagPath);
                await NovelFile.WriteAllBytesAsync(fullPath, flagBytes);
            }
            catch (Exception ex)
            {
                string error = $"[{nameof(FlagsLanguageWorkerEditor)}] Failed to copy flag: {ex.Message}";
                NovelGameDebug.LogError(error);
                result.Message = error;
                result.State = LanguageWorkerState.Error;
                return result;
            }

            return result;
        }
    }
}
