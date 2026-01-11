using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using UnityEngine;
namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.PlayerPrefsSystem
{
    [NodeTint("#3d6b47")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/PlayerPrefs/Delete Key")]
    public class DeleteKeyFromPlayerPrefsNode : BaseNodeInteraction
    {
        [SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private string _key;

        public override void Execute()
        {
            if (PlayerPrefs.HasKey(_key))
            {
                PlayerPrefs.DeleteKey(_key);
            }
        }
    }
}
