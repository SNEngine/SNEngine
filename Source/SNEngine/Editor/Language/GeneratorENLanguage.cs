using Cysharp.Threading.Tasks;
using UnityEditor;

namespace SNEngine.Editor.Language
{
    public static class GeneratorENLanguage
    {
        [MenuItem("SNEngine/Language/Generate English Language")]
        public static void GenerateBlankLanguageMenu()
        {
            _ = GenerateBlankAsync();
        }

        private static async UniTask GenerateBlankAsync()
        {
            try
            {
                await GeneratorLanguage.Generate("en", "en_flag.png");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"[GeneratorENLanguage] Failed to generate blank language: {ex}");
            }
        }
    }
}
