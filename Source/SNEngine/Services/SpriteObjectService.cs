using SNEngine.Polling;
using SNEngine.SpriteObjectSystem;
using SNEngine.Utils;
using UnityEngine;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Sprite Object Service")]
    public class SpriteObjectService : ServiceBase
    {
        private const int COUNT_SPRITE_OBJECTS_ON_START = 15;
        private PoolMono<SpriteObject> _spriteObjects;
        public override void Initialize()
        {
            var prefab = ResourceLoader.LoadCustomOrVanilla<SpriteObject>("Render/SpriteObject");
            var container = new GameObject($"{nameof(SpriteObject)}s");
            _spriteObjects = new(prefab, container.transform, COUNT_SPRITE_OBJECTS_ON_START, true);
            DontDestroyOnLoad(container);
        }

        public ISpriteObject GetSpriteObject ()
        {
            return _spriteObjects.GetFreeElement();
        }

        public override void ResetState()
        {
            foreach (var item in _spriteObjects.Objects)
            {
                item.ResetState();
            }
        }
    }
}
