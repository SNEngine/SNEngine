using UnityEngine;
using UnityEditor;
using System.IO;

namespace SNEngine.Editor
{
    public class SNEPubKeyExtractorLauncher : BaseToolLauncher
    {
        public static void ExtractPublicKey(string gameName, string orgName)
        {
            // Define the output path for the public key in the Resources folder
            string projectPath = Application.dataPath;
            string resourcesPath = Path.Combine(Path.GetDirectoryName(projectPath), "Assets", "SNEngine", "Source", "SNEngine", "Resources");
            string publicKeyOutputPath = Path.Combine(resourcesPath, "sne_public_key.bin");

            // Ensure the Resources directory exists
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
            }

            // Prepare the arguments for the public key extractor
            // Using the format: SNE_PubKeyExtractor <gameName> <orgName> <outputPath>
            string args = $"\"{gameName}\" \"{orgName}\" \"{publicKeyOutputPath}\"";

            // Get the path where the public key extractor executable is located
            string editorFolder = Directory.GetParent(projectPath).FullName;
            string extractorPath = Path.Combine(editorFolder, "Assets", "SNEngine", "Source", "SNEngine", "Editor", "Utils", "SNE_PubKeyExtractor", "Windows");

            // Launch the public key extractor executable using the base class method
            // Looking for the extractor in the Utils/SNE_PubKeyExtractor folder structure
            LaunchExecutable("SNE_PubKeyExtractor", "SNE_PubKeyExtractor.exe", "SNE_PubKeyExtractor", args, (log) => {
                // Log for debugging purposes
                if (log.Contains("[ERROR]"))
                    Debug.LogError($"SNE_PubKeyExtractor: {log}");
                else
                    Debug.Log($"SNE_PubKeyExtractor: {log}");
            });
        }

        public static void VerifyPublicKey(string publicKeyPath, string privateKeyPath)
        {
            // Prepare the arguments for the public key verification
            string args = $"--verify \"{publicKeyPath}\" \"{privateKeyPath}\"";

            // Get the path where the public key extractor executable is located
            string projectPath = Application.dataPath;
            string editorFolder = Directory.GetParent(projectPath).FullName;
            string extractorPath = Path.Combine(editorFolder, "Assets", "SNEngine", "Source", "SNEngine", "Editor", "Utils", "SNE_PubKeyExtractor", "Windows");

            // Launch the public key extractor executable using the base class method for verification
            LaunchExecutable("SNE_PubKeyExtractor", "SNE_PubKeyExtractor.exe", "SNE_PubKeyExtractor", args, (log) => {
                // Log for debugging purposes
#if SNEENGINE_DEVELOPER
                if (log.Contains("[ERROR]") || log.Contains("FAILED"))
                    Debug.LogError($"SNE_PubKeyExtractor: {log}");
                else

                    Debug.Log($"SNE_PubKeyExtractor: {log}");
#endif
            });
        }
    }
}