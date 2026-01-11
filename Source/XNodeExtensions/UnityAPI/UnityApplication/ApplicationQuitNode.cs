
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.UnityApplication
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Application/Quit")]
    public class ApplicationQuitNode : BaseNodeInteraction
    {
        public override void Execute()
        {
            Application.Quit();
        }
    }
}
