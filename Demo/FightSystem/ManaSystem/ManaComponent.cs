using CoreGame.FightSystem.ManaSystem.Models;
using System;
using UnityEngine;

namespace CoreGame.FightSystem.ManaSystem
{
    public class ManaComponent : MonoBehaviour, IManaComponent
    {

        private Mana _manaModel;

        public float CurrentMana => _manaModel.CurrentMana;
        public float MaxMana => _manaModel.MaxMana;

        public event Action<float, float> OnManaChanged;

        public void SetData (float initialMaxMana)
        {
            _manaModel = new Mana(initialMaxMana);

            _manaModel.OnManaChanged += HandleManaChanged;
        }

        private void OnDisable()
        {
            if (_manaModel != null)
            {
                _manaModel.OnManaChanged -= HandleManaChanged;
            }
        }

        private void OnDestroy()
        {
            if (_manaModel != null)
            {
                _manaModel.OnManaChanged -= HandleManaChanged;
            }
        }

        public bool TrySpend(float cost)
        {
            return _manaModel.TrySpend(cost);
        }

        public void Restore(float amount)
        {
            _manaModel.Restore(amount);
        }

        private void HandleManaChanged(float current, float max)
        {
            Debug.Log($"{gameObject.name} Mana: {current} / {max}");
            OnManaChanged?.Invoke(current, max);
        }
    }
}