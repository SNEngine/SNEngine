using SNEngine.Serialisation;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables
{
    public class AudioClipNode : UnityVariableNode<AudioClip, AudioClipLibrary>
    {
        protected override void OnValueChanged(AudioClip oldValue, AudioClip newValue)
        {
            SNEngineSerialization.AddAssetToLibrary<AudioClipLibrary>(newValue);
        }
    }
}
