using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace SNEngine.Animations
{
    public interface IStarWipeable
    {
        UniTask StarWipe(float time, AnimationBehaviourType animationBehaviour, Ease ease);

        UniTask StarWipe(float time, float value, Ease ease);
    }
}
