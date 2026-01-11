using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using SNEngine.Debugging;
namespace SNEngine.Editor
{
    public class AutoIconAssigner : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        private const string ICON_PATH = "Assets/SNEngine/Source/SNEngine/Sprites/default_icon.png";

        public void OnPreprocessBuild(BuildReport report)
        {
            Assign();
        }

        public static void Assign()
        {
            Texture2D[] currentIcons = PlayerSettings.GetIcons(NamedBuildTarget.Unknown, IconKind.Application);

            bool hasIcon = false;
            if (currentIcons != null && currentIcons.Length > 0)
            {
                if (currentIcons[0] != null)
                {
                    hasIcon = true;
                }
            }

            if (!hasIcon)
            {
                Texture2D newIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(ICON_PATH);

                if (newIcon != null)
                {
                    PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new Texture2D[] { newIcon }, IconKind.Application);
                    NovelGameDebug.Log($"[AutoIconAssigner] Icon was missing. Assigned default application icon from: {ICON_PATH}");
                }
                else
                {
                    NovelGameDebug.LogWarning($"[AutoIconAssigner] Icon missing and fallback texture not found at: {ICON_PATH}");
                }
            }
        }
    }
}