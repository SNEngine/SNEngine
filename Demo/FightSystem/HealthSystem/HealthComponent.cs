using UnityEngine;
using System;
using CoreGame.FightSystem.HealthSystem.Models;

namespace CoreGame.FightSystem.HealthSystem
{
    public class HealthComponent : MonoBehaviour, IHealthComponent
    {

       [SerializeField] private Health _healthModel;

        public float CurrentHealth => _healthModel.CurrentHealth;
        public float MaxHealth => _healthModel.MaxHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDied;


        public void SetData(float initialMaxHealth)
        {
            _healthModel = new Health(initialMaxHealth);

            _healthModel.OnHealthChanged += HandleHealthChanged;
            _healthModel.OnDied += HandleDied;
        }

        private void OnDisable()
        {
            if (_healthModel != null)
            {
                _healthModel.OnHealthChanged -= HandleHealthChanged;
                _healthModel.OnDied -= HandleDied;
            }
        }

        private void OnDestroy()
        {
            if (_healthModel != null)
            {
                _healthModel.OnHealthChanged -= HandleHealthChanged;
                _healthModel.OnDied -= HandleDied;
            }
        }


        public void TakeDamage(float damage)
        {
            _healthModel.TakeDamage(damage);
        }

        public void Heal(float amount)
        {
            _healthModel.Heal(amount);
        }


        private void HandleHealthChanged(float current, float max)
        {
            OnHealthChanged?.Invoke(current, max);
        }

        private void HandleDied()
        {
            _healthModel.OnHealthChanged -= HandleHealthChanged;
            _healthModel.OnDied -= HandleDied;
            OnDied?.Invoke();
        }

    }
}