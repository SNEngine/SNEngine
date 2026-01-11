using SNEngine.Serialisation;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables
{
    [NodeTint("#464778")]
    public class SpriteNode : UnityVariableNode<Sprite, SpriteLibrary>
    {
        protected override void OnValueChanged(Sprite oldValue, Sprite newValue)
        {
            SNEngineSerialization.AddAssetToLibrary<SpriteLibrary>(newValue);
        }
    }
}
