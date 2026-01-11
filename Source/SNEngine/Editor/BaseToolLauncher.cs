using UnityEditor;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using System;

namespace SNEngine.Editor
{
    public abstract class BaseToolLauncher
    {
        protected static void LaunchExecutable(string toolFolderName, string windowsExeName, string linuxExeName, string args = "", Action<string> onLogReceived = null)
        {
            string projectPath = Application.dataPath;
            string editorFolder = Directory.GetParent(projectPath).FullName;
            string basePath = $"Assets/SNEngine/Source/SNEngine/Editor/Utils/{toolFolderName}";

            string platformFolder = Application.platform == RuntimePlatform.WindowsEditor ? "Windows" : "Linux";
            string exeName = Application.platform == RuntimePlatform.WindowsEditor ? windowsExeName : linuxExeName;

            string fullPath = Path.Combine(editorFolder, basePath, platformFolder, exeName).Replace('/', Path.DirectorySeparatorChar);

            if (!File.Exists(fullPath))
            {
                onLogReceived?.Invoke($"Error: Executable not found at: {fullPath}");
                return;
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

                process.OutputDataReceived += (sender, e) => {
                    if (e.Data != null) onLogReceived?.Invoke(e.Data);
                };
                process.ErrorDataReceived += (sender, e) => {
                    if (e.Data != null) onLogReceived?.Invoke($"[ERROR] {e.Data}");
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                onLogReceived?.Invoke($"Exception: {e.Message}");
            }
        }
    }
}