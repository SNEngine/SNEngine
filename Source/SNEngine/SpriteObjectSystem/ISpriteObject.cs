using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Animations;
using System.Threading.Tasks;
using UnityEngine;

namespace SNEngine.SpriteObjectSystem
{
    public interface ISpriteObject : IShowable, IHidden, IResetable, IMovableByX, IFadeable, IRotateable, IScaleable, IChangeableColor
    {
        UniTask SetColor(Color inputColor, float inputDuration, Ease inputEase);
        void SetSprite(Sprite inputSprite);
    }
}