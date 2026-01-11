using System;

namespace CoreGame.FightSystem.HealthSystem
{
    public interface IHealthComponent
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        event Action<float, float> OnHealthChanged;
        event Action OnDied;
        void TakeDamage(float damage);
        void Heal(float amount);
        void SetData(float initialMaxHealth);
    }
}