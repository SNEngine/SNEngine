using SNEngine.CharacterSystem;
using SNEngine.Repositories;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Animations;
using SNEngine.Debugging;
using System.Threading.Tasks;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Character Service")]
    public class CharacterService : ServiceBase, IService, IResetable
    {
        private Dictionary<string, ICharacterRenderer> _characters;

        private List<ICharacterRenderer> _charactersInvolved;

        public override void Initialize()
        {
            var characterObject = Resources.Load<CharacterRenderer>("Render/Character");

            _characters = new Dictionary<string, ICharacterRenderer>();

            var characters = NovelGame.Instance.GetRepository<CharacterRepository>().Characters;

            Transform container = new GameObject($"{nameof(Character)}s").transform;

            Object.DontDestroyOnLoad(container);

            foreach (var character in characters)
            {
                var newCharacter = Object.Instantiate(characterObject, container, false);

                newCharacter.Hide();

                newCharacter.SetData(character.Value);

                newCharacter.name = character.Value.OriginalName;

                _characters.Add(character.Value.name, newCharacter);
            }

            _charactersInvolved = new List<ICharacterRenderer>();
        }

        public ICharacterRenderer GetWorldCharacter(Character character)
        {
            if (_characters.TryGetValue(character.OriginalName, out var characterObject))
            {
                return characterObject;
            }

            return null;
        }

        public void ShowCharacter(Character character, string emotionName = "Default")
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            characterRender.ShowWithEmotion(emotionName);
        }

        private bool LogErrorNullReferenceCharacter(Character character)
        {
            if (character is null)
            {
                NovelGameDebug.LogError("character argument is null. Check your node Graph");

                return true;
            }

            return false;

        }

        public void HideCharacter(Character character)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            characterRender.Hide();
        }

        public void HideInvolvedCharacters()
        {
            foreach (var character in _charactersInvolved)
            {
                character.Hide();
            }
        }

        public void ShowInvolvedCharacters()
        {
            foreach (var character in _charactersInvolved)
            {
                character.Show();
            }
        }

        public void HideAllCharacters()
        {
            foreach (var character in _characters)
            {
                character.Value.Hide();
            }
        }

        public void ShowAllCharacters()
        {
            foreach (var character in _characters)
            {
                character.Value.Show();
            }
        }

        public void SetFlipCharacter(Character character, FlipType flipType)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            characterRender.SetFlip(flipType);
        }



        private ICharacterRenderer FindByName(string name)
        {
            ICharacterRenderer character = null;

            if (_characters.ContainsKey(name))
            {
                character = _characters[name];
            }

            else
            {
                NovelGameDebug.LogError($"character with name {name} not founds on db characters");

                return null;
            }

            AddToInvolvedCharacters(character);

            return character;
        }

        private void AddToInvolvedCharacters(ICharacterRenderer character)
        {
            if (!_charactersInvolved.Contains(character))
            {
                _charactersInvolved.Add(character);
            }
        }

        public override void ResetState()
        {
            foreach (var character in _characters.Values)
            {
                character.ResetState();
            }

            _charactersInvolved.Clear();
        }

        #region Animations
        public async UniTask MoveCharacter(Character character, float x, float time, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.MoveX(x, time, ease);
        }

        public async UniTask MoveCharacter(Character character, CharacterDirection direction, float duration, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Move(direction, duration, ease);
        }

        public async UniTask FadeCharacter(Character character, float value, float time, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Fade(value, time, ease);
        }


        public async UniTask FadeCharacter(Character character, AnimationBehaviourType animationBehaviour, float duration, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Fade(duration, animationBehaviour, ease);
        }

        public async UniTask ScaleCharacter(Character character, Vector3 value, float time, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Scale(value, time, ease);
        }

        public async UniTask RotateCharacter(Character character, Vector3 value, float time, Ease ease, RotateMode rotateMode = RotateMode.Fast)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Rotate(value, time, ease, rotateMode);
        }

        public async UniTask ShakePosition(Character character, float duration, float strength = 90, int vibrato = 10, bool fadeOut = true)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.ShakePosition(duration, strength, vibrato, fadeOut);
        }

        public async UniTask SetColorCharacter(Character character, Color value, float time, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.ChangeColor(value, time, ease);
        }

        public async UniTask DissolveCharacter(Character character, AnimationBehaviourType animationBehaviour, float time, Ease ease, Texture2D texture = null)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Dissolve(time, animationBehaviour, ease, texture);
        }

        public async UniTask BlackAndWhiteCharacter(Character character, float value, float duration, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.ToBlackAndWhite(duration, value, ease);
        }

        public async UniTask BlackAndWhiteCharacter(Character character, AnimationBehaviourType animationBehaviour, float duration, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.ToBlackAndWhite(duration, animationBehaviour, ease);
        }

        public async UniTask SolidCharacter(Character character, float value, float duration, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Solid(duration, value, ease);
        }

        public async UniTask SolidCharacter(Character character, AnimationBehaviourType animationBehaviour, float duration, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Solid(duration, animationBehaviour, ease);
        }

        public async UniTask CeliaCharacter(Character character, float value, float duration, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Celia(duration, value, ease);
        }

        public async UniTask CeliaCharacter(Character character, AnimationBehaviourType animationBehaviour, float duration, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Celia(duration, animationBehaviour, ease);
        }

        public async UniTask IlluminateCharacter(Character character, float value, float duration, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Illuminate(duration, value, ease);
        }

        public async UniTask IlluminateCharacter(Character character, AnimationBehaviourType animationBehaviour, float duration, Ease ease)
        {
            if (LogErrorNullReferenceCharacter(character))
            {
                return;
            }

            var characterRender = FindByName(character.name);

            await characterRender.Illuminate(duration, animationBehaviour, ease);
        }

        public Vector3 GetCharacterWorldPosition(Character character)
        {
            var getedCharacter = GetWorldCharacter(character) as CharacterRenderer;
            if (getedCharacter.SpriteIsSeted)
            {
                return getedCharacter.transform.position;
            }

            return Vector3.zero;

        }
        #endregion
    }
}