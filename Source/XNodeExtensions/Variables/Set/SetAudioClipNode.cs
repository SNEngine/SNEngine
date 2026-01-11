using SNEngine.Serialisation;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Set
{
    public class SetAudioClipNode : SetVariableNode<AudioClip>
    {
        protected override void OnSetTargetValueChanged(VariableNode<AudioClip> targetNode, AudioClip newValue)
        {
            SNEngineSerialization.AddAssetToLibrary<AudioClipLibrary>(newValue);
        }
    }
}
