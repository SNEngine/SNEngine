using SNEngine.Serialisation;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables
{
    public class MaterialNode : UnityVariableNode<Material, MaterialLibrary>
    {
        protected override void OnValueChanged(Material oldValue, Material newValue)
        {
            SNEngineSerialization.AddAssetToLibrary<MaterialLibrary>(newValue);
        }
    }
}
