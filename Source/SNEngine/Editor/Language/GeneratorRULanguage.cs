using Cysharp.Threading.Tasks;
using UnityEditor;

namespace SNEngine.Editor.Language
{
    public static class GeneratorRULanguage
    {
        [MenuItem("SNEngine/Language/Generate RU Language")]
        public static void GenerateBlankLanguageMenu()
        {
            _ = GenerateBlankAsync();
        }

        private static async UniTask GenerateBlankAsync()
        {
            try
            {
                await GeneratorLanguage.Generate("ru", "ru_flag.png");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"[GeneratorENLanguage] Failed to generate blank language: {ex}");
            }
        }
    }
}
