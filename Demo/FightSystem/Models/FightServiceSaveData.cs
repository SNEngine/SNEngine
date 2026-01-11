using System;
using CoreGame.FightSystem.Models;

namespace CoreGame.FightSystem.Models
{
    [Serializable]
    public class FightServiceSaveData
    {
        public string PlayerCharacterGUID { get; set; }
        public string EnemyCharacterGUID { get; set; }
        public int CurrentTurnOwner { get; set; }
        public FightCharacterSaveData PlayerData { get; set; }
        public FightCharacterSaveData EnemyData { get; set; }
        public int PlayerEnergyRestoreCounter { get; set; }
        public int EnemyEnergyRestoreCounter { get; set; }
        public bool IsPlayerGuarding { get; set; }
        public bool IsEnemyGuarding { get; set; }
        public FightResult Result { get; set; }

        public FightServiceSaveData()
        {
            PlayerData = new FightCharacterSaveData();
            EnemyData = new FightCharacterSaveData();
        }
    }
}