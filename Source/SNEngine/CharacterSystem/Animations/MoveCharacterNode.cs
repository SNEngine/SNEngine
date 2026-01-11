using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.SaveSystem;
using SNEngine.Services;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SNEngine.CharacterSystem.Animations
{
    public class MoveCharacterNode : AsyncCharacterNode, ISaveProgressNode
    {
        private bool _isLoadFromSaveStub = false;

        [Input(connectionType = ConnectionType.Override), SerializeField] private float _x;

        protected override void Play(Character target, float duration, Ease ease)
        {
            float x = _x;

            if (GetInputPort(nameof(_x)).Connection != null)
            {
                x = GetDataFromPort<float>(nameof(_x));
            }

            float moveDuration = _isLoadFromSaveStub ? 0f : duration;
            Ease moveEase = _isLoadFromSaveStub ? Ease.Unset : ease;

            Move(x, moveDuration, target, moveEase).Forget();
        }

        public override bool CanSkip()
        {
            return false;
        }

        private async UniTask Move(float x, float duration, Character character, Ease ease)
        {
            var serviceCharacters = NovelGame.Instance.GetService<CharacterService>();

            await serviceCharacters.MoveCharacter(character, x, duration, ease);

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