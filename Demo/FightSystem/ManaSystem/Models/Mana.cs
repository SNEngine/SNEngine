using System;
using UnityEngine;

namespace CoreGame.FightSystem.ManaSystem.Models
{
    public class Mana
    {
        private float _currentMana;
        private float _maxMana;

        public event Action<float, float> OnManaChanged;

        public float MaxMana => _maxMana;

        public float CurrentMana
        {
            get => _currentMana;
            private set
            {
                _currentMana = Mathf.Clamp(value, 0, _maxMana);
                OnManaChanged?.Invoke(_currentMana, _maxMana);
            }
        }

        public Mana(float maxMana)
        {
            _maxMana = maxMana;
            _currentMana = maxMana;
        }

        public bool TrySpend(float cost)
        {
            if (cost <= 0 || _currentMana < cost)
            {
                return false;
            }

            CurrentMana -= cost;
            return true;
        }

        public void Restore(float amount)
        {
            if (amount <= 0 || _currentMana >= _maxMana) return;

            CurrentMana += amount;
        }

        public void SetMaxMana(float newMaxMana, bool keepManaRatio = false)
        {
            if (newMaxMana <= 0) return;

            float oldMax = _maxMana;
            float ratio = _currentMana / oldMax;

            _maxMana = newMaxMana;

            if (keepManaRatio)
            {
                CurrentMana = _maxMana * ratio;
            }
            else
            {
                CurrentMana = _currentMana;
            }
        }
    }
}