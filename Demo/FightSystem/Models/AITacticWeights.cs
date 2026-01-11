using System;
using UnityEngine;

namespace CoreGame.FightSystem.Models
{
    [Serializable]
    public class AITacticWeights
    {
        [SerializeField] private float _attackWeight = 40f;
        [SerializeField] private float _guardWeight = 20f;
        [SerializeField] private float _waitWeight = 10f;
        [SerializeField] private float _skillWeight = 30f;

        public float AttackWeight => _attackWeight;
        public float GuardWeight => _guardWeight;
        public float WaitWeight => _waitWeight;
        public float SkillWeight => _skillWeight;
    }

}
