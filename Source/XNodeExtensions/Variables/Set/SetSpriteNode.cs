using SNEngine.Serialisation;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Set
{
    public class SetSpriteNode : SetVariableNode<Sprite>
    {
        protected override void OnSetTargetValueChanged(VariableNode<Sprite> targetNode, Sprite newValue)
        {
            SNEngineSerialization.AddAssetToLibrary<SpriteLibrary>(newValue);
        }
    }
}
