using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
using System;

namespace SNEngine.Security
{
    public static class SNEBootValidator
    {
        // Structure definition to match SNEngineIdentity in C++
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SNEngineIdentity
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] magic;        // 0-3: "SNEI" - магическая подпись
            public uint version;        // 4-7: версия структуры
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] brand;        // 8-39: бренд "MADE_WITH_SNENGINE"
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            public byte[] guid;         // 40-75: традиционный GUID
            public uint mode;           // 76-79: режим (0=text, 1=image)
            public uint dataSize;       // 80-83: размер встроенных данных
            // Данные изображения будут начинаться с 84 байта
        }

        [DllImport("SNE_Validator")]
        private static extern bool ValidateSNE(string executablePath, string platformName, string gameName, string expectedGuid);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Verify()
        {
            if (Application.isEditor) return;

            SNEngineIdentity identity = LoadIdentityFromDatFile();

            // Check if the magic signature is correct to determine if the file was loaded properly
            if (identity.magic == null || identity.magic.Length < 4 ||
                !(identity.magic[0] == 'S' && identity.magic[1] == 'N' && identity.magic[2] == 'E' && identity.magic[3] == 'I'))
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
                    Debug.LogError("SNEngine Security: Could not load project identity from sne_identity.bytes file or invalid magic signature.");
                    Application.Quit();
                    return;
                }
            }

            // Extract GUID string from the identity structure
            string guid = ExtractGuidString(identity.guid);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError("SNEngine Security: Could not extract GUID from identity structure.");
                Application.Quit();
                return;
            }

            // Step 1: Compare raw image data if in image mode
            if (identity.mode == 1 && identity.dataSize > 0)
            {
                bool imageValidationPassed = ValidateImageData(identity);
                if (!imageValidationPassed)
                {
                    Debug.LogError("SNEngine Security: Image validation failed.");
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
                Debug.LogError("SNEngine Security: Validation failed.");
            }
        }

        private static SNEngineIdentity LoadIdentityFromDatFile()
        {
            SNEngineIdentity identity = new SNEngineIdentity();

            // Try to load the .bytes file from Resources
            TextAsset datFile = Resources.Load<TextAsset>("sne_identity");
            if (datFile != null && datFile.bytes.Length > 0)
            {
                byte[] data = datFile.bytes;

                // Check if we have enough data for the basic structure
                if (data.Length >= Marshal.SizeOf(typeof(SNEngineIdentity)))
                {
                    // Extract the basic structure fields
                    identity.magic = new byte[4];
                    Array.Copy(data, 0, identity.magic, 0, 4);

                    identity.version = BitConverter.ToUInt32(data, 4);

                    identity.brand = new byte[32];
                    Array.Copy(data, 8, identity.brand, 0, 32);

                    identity.guid = new byte[36];
                    Array.Copy(data, 40, identity.guid, 0, 36);

                    identity.mode = BitConverter.ToUInt32(data, 76);
                    identity.dataSize = BitConverter.ToUInt32(data, 80);

                    return identity;
                }
            }

            return identity;
        }

        private static string ExtractGuidString(byte[] guidBytes)
        {
            if (guidBytes == null) return null;

            // Convert to string and trim null terminators
            string guid = System.Text.Encoding.UTF8.GetString(guidBytes);
            int nullIndex = guid.IndexOf('\0');
            if (nullIndex >= 0)
                guid = guid.Substring(0, nullIndex);

            return guid.Trim();
        }

        private static bool ValidateImageData(SNEngineIdentity identity)
        {
            // Try to load the original signature image from Resources
            TextAsset originalSignatureAsset = Resources.Load<TextAsset>("original_signature");
            if (originalSignatureAsset == null)
            {
                // If there's no original signature in Resources, skip image validation
                Debug.LogWarning("SNEngine Security: No original signature image found in Resources, skipping image validation.");
                return true;
            }

            // Get the raw image data from the loaded asset
            byte[] originalImageData = originalSignatureAsset.bytes;

            // Load the embedded image data from the sne_identity.bytes file
            TextAsset datFile = Resources.Load<TextAsset>("sne_identity");
            if (datFile == null || datFile.bytes.Length == 0)
            {
                Debug.LogError("SNEngine Security: Could not load sne_identity.bytes file for image comparison.");
                return false;
            }

            byte[] embeddedData = datFile.bytes;
            int imageDataStart = 84; // Data starts after the structure header

            // Calculate how much data we actually have
            int availableDataSize = Math.Min((int)identity.dataSize, embeddedData.Length - imageDataStart);
            if (availableDataSize <= 0)
            {
                Debug.LogError("SNEngine Security: No embedded image data found in identity structure.");
                return false;
            }

            // Extract the embedded image data
            byte[] embeddedImageData = new byte[availableDataSize];
            Array.Copy(embeddedData, imageDataStart, embeddedImageData, 0, availableDataSize);

            // Compare the raw image data
            if (originalImageData.Length != embeddedImageData.Length)
            {
                Debug.LogError($"SNEngine Security: Image size mismatch. Original: {originalImageData.Length}, Embedded: {embeddedImageData.Length}");
                return false;
            }

            // Compare each byte
            for (int i = 0; i < originalImageData.Length; i++)
            {
                if (originalImageData[i] != embeddedImageData[i])
                {
                    Debug.LogError($"SNEngine Security: Image data mismatch at byte {i}. Original: 0x{originalImageData[i]:X2}, Embedded: 0x{embeddedImageData[i]:X2}");
                    return false;
                }
            }

            Debug.Log("SNEngine Security: Image validation passed successfully.");
            return true;
        }
    }
}