using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.PlayerPrefsSystem
{
    [NodeTint("#3d6b47")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/PlayerPrefs/Crear")]
    public class ClearPlayerPrefsNode : BaseNodeInteraction
    {
        public override void Execute()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
