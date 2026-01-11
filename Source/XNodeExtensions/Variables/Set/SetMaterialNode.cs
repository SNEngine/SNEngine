using SNEngine.Serialisation;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Set
{
    public class SetMaterialNode : SetVariableNode<Material> 
    {
        protected override void OnSetTargetValueChanged(VariableNode<Material> targetNode, Material newValue)
        {
            SNEngineSerialization.AddAssetToLibrary<MaterialLibrary>(newValue);
        }
    }
}
