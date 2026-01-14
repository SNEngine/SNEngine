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
            
            // Launch the injector executable using the base class method
            // Looking for the injector in the Utils/SNE_Injector folder structure
            LaunchExecutable("SNE_Injector", "SNE_Injector.exe", "SNE_Injector", args, (log) => {
                // Log for debugging purposes
                if (log.Contains("[ERROR]"))
                    Debug.LogError($"SNE_Injector: {log}");
                else
                    Debug.Log($"SNE_Injector: {log}");
            });
        }
    }
}