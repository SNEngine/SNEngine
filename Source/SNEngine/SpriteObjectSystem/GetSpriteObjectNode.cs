using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Services;
using SNEngine.SpriteObjectSystem;
using UnityEngine;
using XNode;

namespace SNEngine.SpriteObjectSystem
{
    public class GetSpriteObjectNode : BaseNodeInteraction
    {
        [Output(ShowBackingValue.Never), SerializeField] private SpriteObject _result;

        public override void Execute()
        {
            var service = NovelGame.Instance.GetService<SpriteObjectService>();
            _result = service.GetSpriteObject() as SpriteObject;
        }

        public override object GetValue(NodePort port)
        {
            return _result;
        }
    }
}