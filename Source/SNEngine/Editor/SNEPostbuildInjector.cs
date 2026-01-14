using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Diagnostics;
using System.IO;
using SNEngine.Debugging;

namespace SNEngine.Editor
{
    // This class runs after the build is completed to inject security identity
    public class SNEPostbuildInjector : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return int.MaxValue; } } // Run last in the build process
        
        public void OnPostprocessBuild(BuildReport report)
        {
            // Only run for standalone builds (not for the editor)
            if (report.summary.platform == BuildTarget.StandaloneWindows || 
                report.summary.platform == BuildTarget.StandaloneWindows64 ||
                report.summary.platform == BuildTarget.StandaloneLinux64 ||
                report.summary.platform == BuildTarget.StandaloneOSX)
            {
                InjectSecurityIdentity(report);
            }
        }
        
        private static void InjectSecurityIdentity(BuildReport report)
        {
            string projectPath = Application.dataPath;
            string baseDirectory = Directory.GetParent(projectPath).FullName;
            
            // Find the identity files that were generated
            string resourcesPath = Path.Combine(baseDirectory, "Assets", "SNEngine", "Source", "SNEngine", "Resources");
            string identityPath = Path.Combine(resourcesPath, "sne_identity.bytes");
            string configPath = Path.Combine(resourcesPath, "sne_config.txt");

            if (!File.Exists(identityPath))
            {
                NovelGameDebug.LogError("SNEngine Security: Identity file (sne_identity.bytes) not found. Cannot inject security identity.");
                return;
            }

            // Read the project GUID from the identity file
            string projectGuid = null;
            string gameName = null;

            // Try to read from config file first
            if (File.Exists(configPath))
            {
                string[] lines = File.ReadAllLines(configPath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("PROJECT_GUID="))
                    {
                        projectGuid = line.Substring("PROJECT_GUID=".Length);
                    }
                    else if (line.StartsWith("GAME_NAME="))
                    {
                        gameName = line.Substring("GAME_NAME=".Length);
                    }
                }
            }

            // If config file doesn't exist or doesn't contain the required data, try to extract from identity file
            if (string.IsNullOrEmpty(projectGuid) || string.IsNullOrEmpty(gameName))
            {
                // Read the identity file to extract GUID and game name
                if (File.Exists(identityPath))
                {
                    byte[] identityData = File.ReadAllBytes(identityPath);
                    if (identityData.Length >= 76) // Minimum size for SNEngineIdentity structure
                    {
                        // Extract GUID from bytes 40-75 (36 bytes)
                        byte[] guidBytes = new byte[36];
                        System.Array.Copy(identityData, 40, guidBytes, 0, 36);

                        // Convert to string and trim null terminators
                        string extractedGuid = System.Text.Encoding.UTF8.GetString(guidBytes);
                        int nullIndex = extractedGuid.IndexOf('\0');
                        if (nullIndex >= 0)
                            extractedGuid = extractedGuid.Substring(0, nullIndex);

                        if (!string.IsNullOrEmpty(extractedGuid))
                            projectGuid = extractedGuid.Trim();

                        // Extract game name from elsewhere if needed
                        if (string.IsNullOrEmpty(gameName))
                            gameName = PlayerSettings.productName;
                    }
                }
            }
            
            if (string.IsNullOrEmpty(projectGuid) || string.IsNullOrEmpty(gameName))
            {
                NovelGameDebug.LogError("SNEngine Security: Could not read project GUID or game name from config file.");
                return;
            }
            
            // Determine the platform name for the injector
            string platformName = GetPlatformName(report.summary.platform);
            
            // Find the build output directory
            string buildOutputPath = Path.GetDirectoryName(report.summary.outputPath);
            
            // The executable name depends on the platform
            string executableName = GetExecutableName(report.summary.outputPath, report.summary.platform);
            string executablePath = Path.Combine(buildOutputPath, executableName);
            
            if (!File.Exists(executablePath))
            {
                // For some platforms, the executable might be in a subdirectory
                if (report.summary.platform == BuildTarget.StandaloneOSX)
                {
                    // For macOS, the executable is inside the .app bundle
                    string appName = Path.GetFileNameWithoutExtension(report.summary.outputPath);
                    executablePath = Path.Combine(report.summary.outputPath, "Contents", "MacOS", appName);
                }
            }
            
            if (!File.Exists(executablePath))
            {
                NovelGameDebug.LogWarning($"SNEngine Security: Could not find executable at {executablePath}. Skipping injection.");
                return;
            }
            
            // Launch the injector to embed the security identity
            LaunchInjector(executablePath, projectGuid, gameName, platformName);
        }
        
        private static string GetPlatformName(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                    return "StandaloneWindows";
                case BuildTarget.StandaloneWindows64:
                    return "StandaloneWindows64";
                case BuildTarget.StandaloneLinux64:
                    return "StandaloneLinux64";
                case BuildTarget.StandaloneOSX:
                    return "StandaloneOSX";
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                default:
                    return "StandaloneWindows64"; // Default to Windows 64-bit for other platforms
            }
        }
        
        private static string GetExecutableName(string buildPath, BuildTarget buildTarget)
        {
            string fileName = Path.GetFileNameWithoutExtension(buildPath);
            
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return fileName + ".exe";
                case BuildTarget.StandaloneLinux64:
                    return fileName; // Linux executables don't have extensions
                case BuildTarget.StandaloneOSX:
                    return fileName + ".app"; // For macOS, we'll handle the .app bundle separately
                default:
                    return fileName;
            }
        }
        
        private static void LaunchInjector(string executablePath, string projectGuid, string gameName, string platformName)
        {
            // Get the process name to terminate any running instances
            string processName = Path.GetFileNameWithoutExtension(executablePath);

            // Terminate any running instances of the game before injection
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(processName);
            foreach (System.Diagnostics.Process process in processes)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        NovelGameDebug.Log($"SNEngine Security: Terminating running instance of {processName} (PID: {process.Id})");
                        process.Kill();
                        process.WaitForExit(5000); // Wait up to 5 seconds for graceful exit
                    }
                }
                catch (System.Exception ex)
                {
                    NovelGameDebug.LogWarning($"SNEngine Security: Could not terminate process {processName}: {ex.Message}");
                }
            }

            // Small delay to ensure the process is fully terminated
            System.Threading.Thread.Sleep(1000);

            // Use the SNEInjectorLauncher to run the injector
            // The injector will handle terminating any running instances, updating both executable and streaming assets
            SNEInjectorLauncher.InjectIdentity(executablePath, projectGuid, gameName, platformName);

            NovelGameDebug.Log($"SNEngine Security: Successfully injected identity into {executablePath}");
            NovelGameDebug.Log($"SNEngine Security: Unity will now start the game as part of Build and Run process");
        }
    }
}