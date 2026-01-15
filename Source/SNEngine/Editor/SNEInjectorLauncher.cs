using UnityEngine;
using UnityEditor;
using System.IO;

namespace SNEngine.Editor
{
    public class SNEInjectorLauncher : BaseToolLauncher
    {
        public static void InjectIdentity(string executablePath, string projectGuid, string gameName, string platformName)
        {
            // Prepare the arguments for the injector
            string targetPath = Path.GetDirectoryName(executablePath);
            string args = $"\"{targetPath}\" \"{projectGuid}\" \"{gameName}\" \"{platformName}\"";

            // Get the path to the Resources folder to find the identity file
            string projectPath = Application.dataPath;
            string resourcesPath = Path.Combine(Path.GetDirectoryName(projectPath), "Assets", "SNEngine", "Source", "SNEngine", "Resources");
            string identityFilePath = Path.Combine(resourcesPath, "sne_identity.bytes");

            // Get the path where the injector executable is located
            string editorFolder = Directory.GetParent(projectPath).FullName;
            string injectorPath = Path.Combine(editorFolder, "Assets", "SNEngine", "Source", "SNEngine", "Editor", "Utils", "SNE_Injector", "Windows");
            
            // Copy the identity file to the injector's directory so it can be found
            string injectorIdentityPath = Path.Combine(injectorPath, "sne_identity.bytes");
            if (File.Exists(identityFilePath))
            {
                // Use stream-based copying for large files to avoid access issues
                CopyLargeFile(identityFilePath, injectorIdentityPath);
            }
            else
            {
                Debug.LogError($"SNEngine Security: Identity file not found at {identityFilePath}");
                return;
            }

            // Launch the injector executable using the base class method
            // Looking for the injector in the Utils/SNE_Injector folder structure
            LaunchExecutable("SNE_Injector", "SNE_Injector.exe", "SNE_Injector", args, (log) => {
                // Log for debugging purposes
                if (log.Contains("[ERROR]"))
                    Debug.LogError($"SNE_Injector: {log}");
                else
                    Debug.Log($"SNE_Injector: {log}");
            });

            // Optionally remove the copied file after injection (optional cleanup)
            try
            {
                if (File.Exists(injectorIdentityPath))
                {
                    File.Delete(injectorIdentityPath);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not delete temporary identity file: {e.Message}");
            }
        }
        
        private static void CopyLargeFile(string sourcePath, string destinationPath)
        {
            try
            {
                // Ensure the destination directory exists
                string directory = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Check if destination file exists and try to delete it first
                if (File.Exists(destinationPath))
                {
                    try
                    {
                        File.SetAttributes(destinationPath, FileAttributes.Normal); // Remove read-only attribute
                        File.Delete(destinationPath);
                    }
                    catch (System.Exception deleteEx)
                    {
                        Debug.LogWarning($"SNEngine Security: Could not delete existing file {destinationPath}: {deleteEx.Message}");
                    }
                }

                // Small delay to ensure file is released
                System.Threading.Thread.Sleep(100);

                using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 4096, FileOptions.SequentialScan))
                {
                    using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096))
                    {
                        sourceStream.CopyTo(destinationStream);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SNEngine Security: Error copying identity file: {ex.Message}");
                throw; // Re-throw to halt the injection process
            }
        }
    }
}