using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Services;
using System;
using UnityEngine;
using SNEngine.SaveSystem;

namespace SNEngine.CharacterSystem.Animations.BlackAndWhite
{
    public class SetBlackAndWhiteCharacterNode : AsyncCharacterNode, ISaveProgressNode
    {
        private bool _isLoadFromSaveStub = false;

        [Input(connectionType = ConnectionType.Override), Range(0, 1), SerializeField] private float _value;

        protected override void Play(Character target, float duration, Ease ease)
        {
            float value = _value;

            var input = GetInputPort(nameof(_value));

            if (input.Connection != null)
            {
                value = GetDataFromPort<float>(nameof(_value));
            }

            float playDuration = _isLoadFromSaveStub ? 0f : duration;
            Ease playEase = _isLoadFromSaveStub ? Ease.Unset : ease;

            BlackAndWhite(target, value, playDuration, playEase).Forget();
        }

        private async UniTask BlackAndWhite(Character character, float value, float duration, Ease ease)
        {
            var serviceCharacters = NovelGame.Instance.GetService<CharacterService>();

            await serviceCharacters.BlackAndWhiteCharacter(character, value, duration, ease);

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