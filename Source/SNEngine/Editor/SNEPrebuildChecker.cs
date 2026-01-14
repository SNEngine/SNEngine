using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using SNEngine.Debugging;

namespace SNEngine.Editor
{
    public class SNEPrebuildChecker : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return -1000; } } // Run early in the build process

        public void OnPreprocessBuild(BuildReport report)
        {
            // Generate identity files if they don't exist before build
            if (!SNEIdentityGenerator.AreIdentityFilesPresent())
            {
                NovelGameDebug.Log("SNEngine Security: Identity files not found. Generating new identity before build...");
                SNEIdentityGenerator.GenerateIdentity();
            }
            else
            {
                Debug.Log("SNEngine Security: Identity files already exist. Build can proceed.");
            }
        }
    }


}