using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Animations;
using SNEngine.Services;
using SNEngine.BackgroundSystem.AsyncNodes;
using UnityEngine;
using SNEngine.SaveSystem;

namespace SNEngine.BackgroundSystem.Animations.Dissolve
{
    public class DissolveBackgroundNode : DissolveBackgroundNodeBase, ISaveProgressNode
    {
        private bool _isLoadFromSaveStub = false;

        protected override void Play(float duration, AnimationBehaviourType type, Ease ease, Texture2D texture)
        {
            float playDuration = _isLoadFromSaveStub ? 0f : duration;
            Ease playEase = _isLoadFromSaveStub ? Ease.Unset : ease;

            Dissolve(playDuration, type, playEase, texture).Forget();
        }

        private async UniTask Dissolve(float duration, AnimationBehaviourType type, Ease ease, Texture2D texture)
        {
            var backgroundService = NovelGame.Instance.GetService<BackgroundService>();

            await backgroundService.Dissolve(duration, type, ease, texture);

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