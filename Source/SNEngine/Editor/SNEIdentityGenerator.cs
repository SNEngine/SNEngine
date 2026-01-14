using UnityEngine;
using UnityEditor;
using System.IO;
using SNEngine.Debugging;

namespace SNEngine.Editor
{
    [InitializeOnLoad]
    public class SNEIdentityGenerator : BaseToolLauncher
    {
        private const string HIDDEN_IDENTITY_FILE = "._sne_identity.dat";
        private const string CONFIG_FILE = "sne_config.txt";

        static SNEIdentityGenerator()
        {
            // Check for identity file on editor startup
            CheckAndGenerateIdentity();
        }
        public static void CheckAndGenerateIdentity()
        {
            if (!AreIdentityFilesPresent())
            {
                NovelGameDebug.Log("SNEngine Security: Identity files not found. Generating new identity...");

                // Generate new identity
                GenerateIdentity();
            }
            else
            {
                NovelGameDebug.Log("SNEngine Security: Identity files already exist.");
            }
        }

        public static bool AreIdentityFilesPresent()
        {
            string projectPath = Application.dataPath;
            string baseDirectory = Directory.GetParent(projectPath).FullName;

            string resourcesPath = Path.Combine(baseDirectory, "Assets", "SNEngine", "Source", "SNEngine", "Resources");
            string hiddenIdentityPath = Path.Combine(resourcesPath, HIDDEN_IDENTITY_FILE);
            string configPath = Path.Combine(resourcesPath, CONFIG_FILE);

            // Check if identity files exist in the correct location
            bool hasHiddenIdentity = File.Exists(hiddenIdentityPath);
            bool hasConfig = File.Exists(configPath);

            return hasHiddenIdentity && hasConfig;
        }

        public static void GenerateIdentity()
        {
            // Get game name and organization from project settings or use defaults
            string gameName = PlayerSettings.productName;
            if (string.IsNullOrEmpty(gameName))
            {
                gameName = "DefaultGame";
            }

            string organizationName = PlayerSettings.companyName;
            if (string.IsNullOrEmpty(organizationName))
            {
                organizationName = "DefaultStudio";
            }

            // Prepare arguments with output path
            string outputDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Assets", "SNEngine", "Source", "SNEngine", "Resources");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string args = $"\"{gameName}\" \"{organizationName}\" \"{outputDir}\"";

            // Launch the generator executable using the base class method
            // Looking for the generator in the Utils/SNE_Gen folder structure
            // Suppress logs for security reasons
            LaunchExecutable("SNE_Gen", "SNE_Gen.exe", "SNE_Gen", args);
        }
    }
}