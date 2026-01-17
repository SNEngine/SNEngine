using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

namespace SNEngine.Editor
{
    // This class runs before the build is started to ensure public key is generated
    // This ensures that the public key is available when the post-build injector runs
    public class SNEPrebuildKeyGenerator : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return int.MinValue; } } // Run first in the build process

        public void OnPreprocessBuild(BuildReport report)
        {
            // Generate public key for the security system before build
            // This ensures the public key is available when the post-build injector runs
            string productName = string.IsNullOrEmpty(PlayerSettings.productName) ? "MyGame" : PlayerSettings.productName;
            string companyName = PlayerSettings.companyName;

            // Check if company name is default and use fallback
            if (string.IsNullOrEmpty(companyName) || companyName == "DefaultCompany")
            {
                SNEPubKeyExtractorLauncher.ExtractPublicKey(productName, "MyOrganization");
            }
            else
            {
                SNEPubKeyExtractorLauncher.ExtractPublicKey(productName, companyName);
            }
        }
    }
}