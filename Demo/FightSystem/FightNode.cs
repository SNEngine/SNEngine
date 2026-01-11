using CoreGame.FightSystem.Models;
using CoreGame.FightSystem.UI;
using CoreGame.Services;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine;
using SNEngine.SaveSystem;
using System;
using UnityEngine;
using XNode;
namespace CoreGame.FightSystem
{
    public class FightNode : AsyncNode, ISaveProgressNode
    {
        private const float DELAY_FINISH_FIGHT = 0.2f;
        [SerializeField] private FightCharacter _playerCharacter;
        [SerializeField] private FightCharacter _enemyCharacter;
        [Output(ShowBackingValue.Never), SerializeField, Header("Victory - 0 Defeat - 1 Tie - 2")] private int _result;
        private bool _finished;
        private FightServiceSaveData _saveData;

        public override void Execute()
        {
            if (_finished)
            {
                return;
            }
            if (!_playerCharacter)
            {
                Debug.LogError($"fight player character not seted to node {GUID}");
                return;
            }

            if (!_enemyCharacter)
            {
                Debug.LogError($"fight enemy character not seted to node {GUID}");
                return;
            }
            base.Execute();

            var fightService = NovelGame.Instance.GetService<FightService>();
            fightService.OnFightEnded += OnFightEnded;
            fightService.TurnFight(_playerCharacter, _enemyCharacter, _saveData);
        }

        private async void OnFightEnded(FightResult result)
        {
            var fightService = NovelGame.Instance.GetService<FightService>();
            fightService.OnFightEnded -= OnFightEnded;
            _result = (int)result;
            _finished = true;
            await UniTask.WaitForSeconds(FightWindow.ANIMATION_DURATION_HIDE + DELAY_FINISH_FIGHT);
            StopTask();
        }

        public override bool CanSkip()
        {
            return false;
        }

        public object GetDataForSave()
        {
            return NovelGame.Instance.GetService<FightService>().GetSaveData();
        }

        public void ResetSaveBehaviour()
        {
            _saveData = null;
            _finished = false;
            _result = 0;
        }

        public void SetDataFromSave(object data)
        {
            if (data is JObject jObject)
            {
                try
                {
                    FightServiceSaveData saveData = jObject.ToObject<FightServiceSaveData>();
                    if (saveData != null)
                    {
                        _saveData = saveData;
                        _finished = _saveData.Result != FightResult.None;
                        _result = (int)saveData.Result;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Deserialization error: {e.Message}");
                }
            }
            else if (data is FightServiceSaveData directSaveData)
            {
                _saveData = directSaveData;
                _finished = _saveData.Result != FightResult.None;
                _result = (int)directSaveData.Result;
            }
        }

        public override object GetValue(NodePort port)
        {
            return _result;
        }
    }
}