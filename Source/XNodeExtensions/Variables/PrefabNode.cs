using SNEngine.Serialisation;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables
{
    [NodeTint("#524c49")]
    public class PrefabNode : UnityVariableNode<GameObject, PrefabLibrary>
    {
        protected override void OnValueChanged(GameObject oldValue, GameObject newValue)
        {
            SNEngineSerialization.AddAssetToLibrary<PrefabLibrary>(newValue);
        }
    }
}
