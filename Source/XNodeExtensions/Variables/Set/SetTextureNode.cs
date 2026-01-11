using SNEngine.Serialisation;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Set
{
    public class SetTextureNode : SetVariableNode<Texture>
    {
        [Input(ShowBackingValue.Never), SerializeField] private Texture2D _inputTexture;

        public override void Execute()
        {
            Texture2D textureFromPort = GetDataFromPort<Texture2D>(nameof(_inputTexture));
            
            if (textureFromPort != null)
            {
                Value = textureFromPort;
            }

            base.Execute();
        }

        protected override void OnSetTargetValueChanged(VariableNode<Texture> targetNode, Texture newValue)
        {
            if (newValue != null)
            {
                SNEngineSerialization.AddAssetToLibrary<TextureLibrary>(newValue);
            }
        }
    }
}