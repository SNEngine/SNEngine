using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace SNEngine.Animations
{
    public interface IVignettable
    {
        UniTask Vignette(float time, AnimationBehaviourType animationBehaviour, Ease ease);

        UniTask Vignette(float time, float value, Ease ease);
    }
}
