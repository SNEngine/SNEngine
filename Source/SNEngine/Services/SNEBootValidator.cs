using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;

namespace SNEngine.Security
{
    public static class SNEBootValidator
    {

        [DllImport("SNE_Validator")]
        private static extern bool ValidateSNE(string executablePath, string platformName, string gameName, string expectedGuid);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Verify()
        {
            if (Application.isEditor) return;
            string guid = LoadGuidFromDatFile();
            if (string.IsNullOrEmpty(guid))
            {
                // Fallback to a default GUID if the file is not found
                // In editor, we might not have the file, so we can skip validation
                if (Application.isEditor)
                {
                    Debug.Log("SNEngine Security: Running in editor, skipping validation.");
                    return;
                }
                else
                {
                    Debug.LogError("SNEngine Security: Could not load project GUID from sne_identity.bytes file.");
                    Application.Quit();
                    return;
                }
            }

            string gameName = Application.productName;
            if (string.IsNullOrEmpty(gameName))
                gameName = "DefaultGame";

            string platformName = "";
            string executablePath = "";

#if UNITY_STANDALONE_WIN
            platformName = "Windows";
            executablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
#elif UNITY_STANDALONE_LINUX
            platformName = "Linux";
            executablePath = "/proc/self/exe";
#elif UNITY_STANDALONE_OSX
            platformName = "OSX";
            executablePath = Application.dataPath + "/../" + Application.productName + ".app/Contents/MacOS/" + Application.productName;
#elif UNITY_ANDROID
            platformName = "Android";
            executablePath = Application.dataPath + "/libSNE_Validator.so";
#else
            platformName = "Unknown";
            executablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
#endif

            if (!ValidateSNE(executablePath, platformName, gameName, guid))
            {
            }
        }

        private static string LoadGuidFromDatFile()
        {
            // Try to load the .bytes file from Resources
            TextAsset datFile = Resources.Load<TextAsset>("sne_identity");
            if (datFile != null && datFile.bytes.Length > 0)
            {
                // The SNEIdentity structure has the GUID at position 36-72 (after magic and brand)
                byte[] data = datFile.bytes;
                if (data.Length >= 72) // Minimum size for SNEIdentity
                {
                    // Extract the GUID from the binary data (bytes 36-71)
                    byte[] guidBytes = new byte[36];
                    System.Array.Copy(data, 36, guidBytes, 0, 36);

                    // Convert to string and trim null terminators
                    string guid = System.Text.Encoding.UTF8.GetString(guidBytes);
                    int nullIndex = guid.IndexOf('\0');
                    if (nullIndex >= 0)
                        guid = guid.Substring(0, nullIndex);

                    return guid.Trim();
                }
            }

            return null;
        }
    }
}