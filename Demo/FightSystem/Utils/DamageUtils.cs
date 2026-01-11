using CoreGame.FightSystem.Models;
using UnityEngine;

namespace CoreGame.FightSystem.Utils
{
    public static class DamageUtils
    {
        public static float ApplyGuardReduction(FightCharacter target, float damage)
        {
            return damage * (1f - target.GuardReductionPercentage);
        }
    }
}
