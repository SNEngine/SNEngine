using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Animations;
using SNEngine.Services;
using UnityEngine;
using SNEngine.SaveSystem;

namespace SNEngine.CharacterSystem.Animations.Celia
{
    public class CeliaCharacterInOutNode : AnimationInOutNode<Character>, ISaveProgressNode
    {
        private bool _isLoadFromSaveStub = false;

        protected override void Play(Character target, float duration, AnimationBehaviourType type, Ease ease)
        {
            float playDuration = _isLoadFromSaveStub ? 0f : duration;
            Ease playEase = _isLoadFromSaveStub ? Ease.Unset : ease;

            Celia(target, type, playDuration, playEase).Forget();
        }

        private async UniTask Celia(Character character, AnimationBehaviourType animationBehaviour, float duration, Ease ease)
        {
            var serviceCharacters = NovelGame.Instance.GetService<CharacterService>();

            await serviceCharacters.CeliaCharacter(character, animationBehaviour, duration, ease);

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