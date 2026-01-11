using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Animations;
using UnityEngine;

namespace SNEngine.CharacterSystem
{
    public interface ICharacterRenderer : IShowable, IHidden, IResetable, IMovableByX, IFadeable, IRotateable, IScaleable, IChangeableColor, IDissolveable, IBlackAndWhiteSupport, ICeliable, ISolidable, IIlluminatiionable, IFlipable
    {
        bool SpriteIsSeted { get; }
        void ShowWithEmotion(string emotionName);

        UniTask Move(CharacterDirection direction, float time, Ease ease);
        T AddComponent<T>() where T : Component;
        UniTask ShakePosition(float duration, float strength = 90, int vibrato = 10, bool fadeOut = true);

    }
}