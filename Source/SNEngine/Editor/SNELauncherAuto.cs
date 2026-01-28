using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace SNEngine.Editor
{
    [InitializeOnLoad]
    public class SNELauncherAuto
    {
        static SNELauncherAuto()
        {
            // Автоматически регистрируем делегат для запуска лаунчера при старте редактора
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                // Запускаем лаунчер перед запуском игры в режиме play
                LaunchLauncher();
            }
        }

        private static void LaunchLauncher()
        {
            // Проверяем, включена ли опция автозапуска
            if (!EditorPrefs.GetBool("SNE_Launcher_AutoRun", false))
                return;

            // Запускаем лаунчер
            string projectPath = Application.dataPath;
            string editorFolder = Directory.GetParent(projectPath).FullName;
            string launcherPath = Path.Combine(editorFolder, "Assets", "SNEngine", "Source", "SNEngine", "Editor", "Utils", "SNE_Injector", "Windows", "SNE_Launcher.exe");

            if (File.Exists(launcherPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(launcherPath)
                {
                    Arguments = "--fullscreen", // Запускаем в полноэкранном режиме
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Directory.GetParent(launcherPath).FullName
                };

                Process.Start(startInfo);

                // Ждем немного, чтобы лаунчер успел запуститься
                System.Threading.Thread.Sleep(1000);
            }
            else
            {
                Debug.LogWarning("SNE_Launcher.exe not found at: " + launcherPath);
            }
        }

        [MenuItem("SNEngine/Launcher/Auto Run Enabled", true)] // Горячая клавиша для проверки состояния
        public static bool ToggleAutoRunValidate()
        {
            Menu.SetChecked("SNEngine/Launcher/Auto Run Enabled", EditorPrefs.GetBool("SNE_Launcher_AutoRun", false));
            return true;
        }

        [MenuItem("SNEngine/Launcher/Auto Run Enabled")] // Переключатель для опции автозапуска
        public static void ToggleAutoRun()
        {
            bool currentValue = EditorPrefs.GetBool("SNE_Launcher_AutoRun", false);
            EditorPrefs.SetBool("SNE_Launcher_AutoRun", !currentValue);
            Debug.Log("SNE_Launcher Auto Run: " + (!currentValue ? "ENABLED" : "DISABLED"));
        }
    }
}