using SNEngine.Serialisation;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Set
{
    [NodeTint("#4a5052")]
    public class SetPrefabNode : SetVariableNode<GameObject>
    {
        protected override void OnSetTargetValueChanged(VariableNode<GameObject> targetNode, GameObject newValue)
        {
            SNEngineSerialization.AddAssetToLibrary<PrefabLibrary>(newValue);
        }
    }
}
