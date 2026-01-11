using SNEngine.Serialisation;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Textures
{
    public class TextureNode : UnityVariableNode<Texture, TextureLibrary>
    {
        protected override void OnValueChanged(Texture oldValue, Texture newValue)
        {
            SNEngineSerialization.AddAssetToLibrary<TextureLibrary>(newValue);
        }
    }
}
