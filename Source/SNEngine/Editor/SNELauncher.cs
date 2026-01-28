using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

namespace SNEngine.Editor
{
    public class SNELauncher : BaseToolLauncher
    {
        /// <summary>
        /// Запускает SNE_Launcher.exe с указанными аргументами
        /// </summary>
        /// <param name="args">Аргументы командной строки для SNE_Launcher.exe</param>
        public static void Launch(string args = "")
        {
            LaunchExecutable("SNE_Injector", "SNE_Launcher.exe", "SNE_Launcher", args, (log) => {
                if (log.Contains("[ERROR]"))
                    UnityEngine.Debug.LogError($"SNE_Launcher: {log}");
                else
                    UnityEngine.Debug.Log($"SNE_Launcher: {log}");
            });
        }

        /// <summary>
        /// Запускает SNE_Launcher.exe и возвращает код выхода
        /// </summary>
        /// <param name="args">Аргументы командной строки для SNE_Launcher.exe</param>
        /// <returns>Код выхода процесса лаунчера</returns>
        public static int LaunchAndWaitForResult(string args = "")
        {
            string projectPath = Application.dataPath;
            string editorFolder = Directory.GetParent(projectPath).FullName;
            string basePath = $"Assets/SNEngine/Source/SNEngine/Editor/Utils/SNE_Injector";

            string platformFolder = Application.platform == RuntimePlatform.WindowsEditor ? "Windows" : "Linux";
            string exeName = Application.platform == RuntimePlatform.WindowsEditor ? "SNE_Launcher.exe" : "SNE_Launcher";

            string fullPath = Path.Combine(editorFolder, basePath, platformFolder, exeName).Replace('/', Path.DirectorySeparatorChar);

            if (!File.Exists(fullPath))
            {
                UnityEngine.Debug.LogError($"SNE_Launcher: Executable not found at: {fullPath}");
                return -1;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(fullPath)
                {
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Directory.GetParent(fullPath).FullName
                };

                Process process = new Process { StartInfo = startInfo };

                // Capture output
                string output = "";
                string error = "";

                process.OutputDataReceived += (sender, e) => {
                    if (e.Data != null) {
                        output += e.Data + "\n";
                        UnityEngine.Debug.Log($"SNE_Launcher: {e.Data}");
                    }
                };
                process.ErrorDataReceived += (sender, e) => {
                    if (e.Data != null) {
                        error += e.Data + "\n";
                        UnityEngine.Debug.LogError($"SNE_Launcher: {e.Data}");
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(); // Wait for the process to complete

                int exitCode = process.ExitCode;

                // When the launcher successfully completes (user accepted license),
                // we mark the license as accepted in Unity's EditorPrefs
                if (exitCode == 0) // Successful exit typically means license was accepted
                {
                    string productName = string.IsNullOrEmpty(PlayerSettings.productName) ? "default_game" : PlayerSettings.productName;
                    string companyName = PlayerSettings.companyName;
                    string gameIdentifier = string.IsNullOrEmpty(companyName) ? productName : $"{companyName}.{productName}";
                    EditorPrefs.SetBool($"SNE_LicenseAccepted_{gameIdentifier}", true);
                }

                return exitCode;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"SNE_Launcher: Exception: {e.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Запускает SNE_Launcher.exe в полноэкранном режиме
        /// </summary>
        public static int LaunchFullscreen()
        {
            return LaunchAndWaitForResult("--fullscreen");
        }

        /// <summary>
        /// Запускает SNE_Launcher.exe с указанным изображением сплэша и временем отображения
        /// </summary>
        /// <param name="splashImagePath">Путь к изображению сплэша</param>
        /// <param name="durationMs">Время отображения сплэша в миллисекундах</param>
        /// <param name="fullscreen">Запускать в полноэкранном режиме</param>
        public static int LaunchWithSplash(string splashImagePath = "sne_splash.png", int durationMs = 3000, bool fullscreen = false)
        {
            string args = $"\"{splashImagePath}\" {durationMs}";
            if (fullscreen)
                args += " --fullscreen";

            return LaunchAndWaitForResult(args);
        }
    }
}