using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Animations;
using UnityEngine;

namespace SNEngine.BackgroundSystem
{
    public interface IBackgroundRenderer : IIlluminatiionable, ICeliable, IBlackAndWhiteSupport, ISolidable, IDissolveable, IStarWipeable, IVignettable
    {
        bool UseTransition { get; set; }

        void Clear();
        void ResetState();
        void SetData(Sprite sprite);

        // Animations
        UniTask SetTransperent(float fadeValue, float duration, Ease ease);
        UniTask SetColor(Color color, float duration, Ease ease);
        UniTask SetBrightness(float brightnessValue, float duration, Ease ease);

        UniTask MoveTo(Vector3 position, float duration, Ease ease);
        UniTask LocalMoveTo(Vector3 localPosition, float duration, Ease ease);
        UniTask RotateTo(Vector3 rotation, float duration, Ease ease);
        UniTask LocalRotateTo(Vector3 localRotation, float duration, Ease ease);
        UniTask ScaleTo(Vector3 scale, float duration, Ease ease);

        UniTask PunchPosition(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1);
        UniTask PunchRotation(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1);
        UniTask PunchScale(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1);

        UniTask ShakePosition(float duration, float strength = 90, int vibrato = 10, bool fadeOut = true);
        UniTask ShakeRotation(float duration, float strength = 90, int vibrato = 10, bool fadeOut = true);
        UniTask ShakeScale(float duration, float strength = 1, int vibrato = 10, float fadeOut = 0);

        UniTask MoveOnPath(Vector3[] path, float duration, PathType pathType = PathType.CatmullRom, Ease ease = Ease.Linear);
        UniTask LookAtTarget(Vector3 worldPosition, float duration, Ease ease);

        void SetLoopingMove(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear);
        void SetLoopingRotate(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear);
        void SetLoopingScale(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear);
    }
}