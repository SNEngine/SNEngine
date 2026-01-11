using SNEngine.Serialisation;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Set
{
    public class SetTexture2DNode : SetVariableNode<Texture2D>
    {
        protected override void OnSetTargetValueChanged(VariableNode<Texture2D> targetNode, Texture2D newValue)
        {
            SNEngineSerialization.AddAssetToLibrary<Texture2DLibrary>(newValue);
        }
    }
}
