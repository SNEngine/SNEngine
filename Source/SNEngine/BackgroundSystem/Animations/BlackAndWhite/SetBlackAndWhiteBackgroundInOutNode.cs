using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Animations;
using SNEngine.BackgroundSystem.AsyncNodes;
using SNEngine.Services;
using XNode;
using SNEngine.SaveSystem;

namespace SNEngine.BackgroundSystem.Animations.BlackAndWhite
{
    public class SetBlackAndWhiteBackgroundInOutNode : AsyncBackgroundInOutNode, ISaveProgressNode
    {
        private bool _isLoadFromSaveStub = false;

        protected override void Play(float duration, AnimationBehaviourType type, Ease ease)
        {
            float playDuration = _isLoadFromSaveStub ? 0f : duration;
            Ease playEase = _isLoadFromSaveStub ? Ease.Unset : ease;

            BlackAndWhite(type, playDuration, playEase).Forget();
        }

        private async UniTask BlackAndWhite(AnimationBehaviourType animationBehaviour, float duration, Ease ease)
        {
            var backgroundService = NovelGame.Instance.GetService<BackgroundService>();

            await backgroundService.ToBlackAndWhite(duration, animationBehaviour, ease);

            StopTask();
        }

        public object GetDataForSave()
        {
            return null;
        }

        public void SetDataFromSave(object data)
        {
            _isLoadFromSaveStub = true;
        }

        public void ResetSaveBehaviour()
        {
            _isLoadFromSaveStub = false;
        }
    }
}