using UnityEditor;
using Cysharp.Threading.Tasks;

namespace SNEngine.Editor.Language
{
    public static class GeneratorBlankLanguage
    {
        [MenuItem("SNEngine/Language/Generate Blank Language")]
        public static void GenerateBlankLanguageMenu()
        {
            _ = GenerateBlankAsync();
        }

        private static async UniTask GenerateBlankAsync()
        {
            try
            {
                await GeneratorLanguage.Generate("blank");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"[GeneratorBlankLanguage] Failed to generate blank language: {ex}");
            }
        }
    }
}
