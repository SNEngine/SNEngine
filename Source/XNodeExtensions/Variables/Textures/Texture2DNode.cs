using SNEngine.Serialisation;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Textures
{
    public class Texture2DNode : UnityVariableNode<Texture2D, Texture2DLibrary>
    {
        protected override void OnValueChanged(Texture2D oldValue, Texture2D newValue)
        {
            SNEngineSerialization.AddAssetToLibrary<Texture2DLibrary>(newValue);
        }
    }
}
