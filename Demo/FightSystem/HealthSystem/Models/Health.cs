using System;
using UnityEngine;

namespace CoreGame.FightSystem.HealthSystem.Models
{
    [Serializable]
    public class Health
    {
        [SerializeField]  private float _currentHealth;
        [SerializeField] private float _maxHealth;

        public event Action<float, float> OnHealthChanged;

        public event Action OnDamaged;

        public event Action OnDied;

        public float MaxHealth => _maxHealth;

        public float CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
                OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

                if (_currentHealth <= 0)
                {
                    OnDied?.Invoke();
                }
            }
        }

        public Health(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (damage <= 0 || _currentHealth <= 0) return;

            CurrentHealth -= damage;
            OnDamaged?.Invoke();
        }

        public void Heal(float amount)
        {
            if (amount <= 0 || _currentHealth >= _maxHealth) return;

            CurrentHealth += amount;
        }

        public void SetMaxHealth(float newMaxHealth, bool keepHealthRatio = false)
        {
            if (newMaxHealth <= 0) return;

            float oldMax = _maxHealth;
            float ratio = _currentHealth / oldMax;

            _maxHealth = newMaxHealth;

            if (keepHealthRatio)
            {
                CurrentHealth = _maxHealth * ratio;
            }
            else
            {
                CurrentHealth = _currentHealth;
            }
        }
    }
}