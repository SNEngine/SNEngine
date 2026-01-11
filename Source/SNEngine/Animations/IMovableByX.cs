using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace SNEngine.Animations
{
    public interface IMovableByX
    {
        UniTask MoveX(float x, float time, Ease ease);
        UniTask MoveY(float y, float time, Ease ease);
    }
}
