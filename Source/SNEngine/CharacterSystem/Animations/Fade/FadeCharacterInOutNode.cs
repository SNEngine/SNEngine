using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Animations;
using SNEngine.Services;
using UnityEngine;
using SNEngine.SaveSystem;

namespace SNEngine.CharacterSystem.Animations.Fade
{
    public class FadeCharacterInOutNode : AnimationInOutNode<Character>, ISaveProgressNode
    {
        private bool _isLoadFromSaveStub = false;

        [Input(connectionType = ConnectionType.Override), Range(0, 1), SerializeField] private float _value;

        protected override void Play(Character target, float duration, AnimationBehaviourType type, Ease ease)
        {
            float value = _value;

            var input = GetInputPort(nameof(_value));

            if (input.Connection != null)
            {
                value = GetDataFromPort<float>(nameof(_value));
            }

            float playDuration = _isLoadFromSaveStub ? 0f : duration;
            Ease playEase = _isLoadFromSaveStub ? Ease.Unset : ease;

            Fade(target, type, playDuration, playEase).Forget();
        }

        private async UniTask Fade(Character character, AnimationBehaviourType animationBehaviour, float duration, Ease ease)
        {
            var serviceCharacters = NovelGame.Instance.GetService<CharacterService>();

            await serviceCharacters.FadeCharacter(character, animationBehaviour, duration, ease);

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