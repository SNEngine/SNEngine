using XNode;
using UnityEngine;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.PlayerPrefsSystem
{
    [NodeTint("#3d6b47")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/PlayerPrefs/Has Key")]
    public class HasKeyOnPlayerPrefs : BaseNode
    {
        [Input, SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private string _key;

        [Output(ShowBackingValue.Never), SerializeField] private bool _result;
        public override object GetValue(NodePort port)
        {
            var currentKey = _key;

            var keyInput = GetDataFromPort<string>(nameof(_key));

            if (keyInput != null)
            {
                currentKey = keyInput;
            }

            return PlayerPrefs.HasKey(currentKey);
        }
    }
}
