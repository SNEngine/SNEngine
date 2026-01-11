using System;

namespace CoreGame.FightSystem.ManaSystem
{
    public interface IManaComponent
    {
        public event Action<float, float> OnManaChanged;
        public float CurrentMana { get; }
        public float MaxMana { get; }
        bool TrySpend(float cost);
        void Restore(float amount);
        void SetData(float initialMaxMana);
    }
}