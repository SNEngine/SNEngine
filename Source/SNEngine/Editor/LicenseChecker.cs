using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Linq;

namespace SNEngine.Editor
{
    public class LicenseChecker
    {
#if UNITY_EDITOR_WIN
        // Windows registry constants
        private const string REGISTRY_BASE_PATH = @"SOFTWARE\SNENGINE\Games";
        private const string REGISTRY_DEVICE_GUID_VALUE = "DeviceGUID";
        private const string REGISTRY_LICENSE_VERSION_VALUE = "LicenseVersion";
        private const string REGISTRY_GAME_IDENTIFIER_VALUE = "GameIdentifier";
#endif

        // Method to check if license agreement has been accepted
        public static bool HasLicenseBeenAccepted(string gameIdentifier = "default_game")
        {
            // Try to read license info from storage (registry on Windows, file on other platforms)
            var licenseInfo = ReadLicenseInfoFromStorage(gameIdentifier);
            
            if (string.IsNullOrEmpty(licenseInfo.deviceGuid) || string.IsNullOrEmpty(licenseInfo.licenseVersion))
            {
                // No license info stored, meaning agreement hasn't been accepted
                return false;
            }

            // Compare stored version with current license version
            string currentLicenseVersion = GetCurrentLicenseVersion(gameIdentifier);
            var storedVersion = ParseVersion(licenseInfo.licenseVersion);
            var currentVersion = ParseVersion(currentLicenseVersion);

            // If stored version is greater than or equal to current version, license is accepted
            // If stored version is less than current version, user needs to accept new terms
            return (storedVersion.major > currentVersion.major) || 
                   (storedVersion.major == currentVersion.major && storedVersion.minor >= currentVersion.minor);
        }

        // Read license info from storage (registry on Windows, file on other platforms)
        private static (string deviceGuid, string licenseVersion, string gameIdentifier) ReadLicenseInfoFromStorage(string gameIdentifier)
        {
#if UNITY_EDITOR_WIN
            // On Windows, read from registry
            string registryPath = $"{REGISTRY_BASE_PATH}\\{gameIdentifier}";
            
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath))
                {
                    if (key != null)
                    {
                        string guid = key.GetValue(REGISTRY_DEVICE_GUID_VALUE)?.ToString() ?? "";
                        string version = key.GetValue(REGISTRY_LICENSE_VERSION_VALUE)?.ToString() ?? "";
                        return (guid, version, gameIdentifier);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error reading license info from registry: {e.Message}");
            }
            
            return ("", "", gameIdentifier);
#else
            // On non-Windows platforms, read from config file
            string configPath = GetConfigPath(gameIdentifier);
            
            if (File.Exists(configPath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(configPath);
                    if (lines.Length >= 3)
                    {
                        return (lines[0], lines[1], lines[2]);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error reading license info from file: {e.Message}");
                }
            }
            
            return ("", "", gameIdentifier);
#endif
        }

        // Get current license version from license file
        private static string GetCurrentLicenseVersion(string gameIdentifier)
        {
            // Determine the language to load the appropriate license file
            string detectedLanguage = GetSystemLanguage();
            string licensePath = Path.Combine(Application.dataPath, "..", "Assets", "SNEngine", "Source", "SNEngine", "Editor", "Utils", "SNE_Injector", "Windows", "licenses", $"LICENSE_{detectedLanguage.ToUpper()}");

            if (!File.Exists(licensePath))
            {
                // Fallback to English if specific language file doesn't exist
                licensePath = Path.Combine(Application.dataPath, "..", "Assets", "SNEngine", "Source", "SNEngine", "Editor", "Utils", "SNE_Injector", "Windows", "licenses", "LICENSE_EN");
            }

            if (File.Exists(licensePath))
            {
                try
                {
                    string firstLine = File.ReadLines(licensePath).First();
                    // Look for version pattern like "vX.X-" in the line
                    int startPos = firstLine.IndexOf('v');
                    if (startPos != -1)
                    {
                        int endPos = firstLine.IndexOf('-', startPos);
                        if (endPos != -1)
                        {
                            string version = firstLine.Substring(startPos, endPos - startPos);
                            // Validate that we have a proper version format
                            if (version.Contains('.'))
                            {
                                return version;
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error reading current license version: {e.Message}");
                }
            }

            return "unknown";
        }

        // Parse version string into numeric components
        private static (int major, int minor) ParseVersion(string versionStr)
        {
            if (versionStr == "unknown")
            {
                return (0, 0);
            }

            // Expected format: vX.Y (e.g., v2.1)
            int dotPos = versionStr.IndexOf('.');
            if (dotPos != -1)
            {
                try
                {
                    int major = int.Parse(versionStr.Substring(1, dotPos - 1)); // Skip 'v'
                    int minor = int.Parse(versionStr.Substring(dotPos + 1));
                    return (major, minor);
                }
                catch (System.Exception)
                {
                    // If parsing fails, return 0.0
                    return (0, 0);
                }
            }
            
            return (0, 0);
        }

        // Get system language
        private static string GetSystemLanguage()
        {
            // This is a simplified version - in a real implementation you might want to use
            // a more sophisticated method to detect the system language
            System.Globalization.CultureInfo currentCulture = System.Globalization.CultureInfo.CurrentCulture;
            string twoLetterLang = currentCulture.TwoLetterISOLanguageName.ToUpper();

            // Map some common languages to our expected codes
            switch (twoLetterLang)
            {
                case "EN":
                    return "EN";
                case "RU":
                    return "RU";
                case "AR":
                    return "AR";
                case "JA":
                    return "JA";
                case "ZH":
                    return "ZH";
                default:
                    return "EN"; // Default to English
            }
        }

#if !UNITY_EDITOR_WIN
        // Get config file path on non-Windows platforms
        private static string GetConfigPath(string gameIdentifier)
        {
            string configDir = GetConfigDir();
            return Path.Combine(configDir, $".snengine_hwid_{gameIdentifier}");
        }

        // Get config directory path on non-Windows platforms
        private static string GetConfigDir()
        {
            string home = System.Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrEmpty(home))
            {
                return Path.Combine(home, ".config");
            }
            return "."; // fallback
        }
#endif
    }
}