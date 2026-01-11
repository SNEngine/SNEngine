using CoreGame.FightSystem.Abilities;
using CoreGame.FightSystem.UI.Markers;
using CoreGame.Services;
using SNEngine;
using SNEngine.Polling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame.FightSystem.UI
{
    public class AbilityWindow : MonoBehaviour
    {
        [SerializeField] private AbilityText _prefab;
        [SerializeField] private RectTransform _containerAbility;

        [SerializeField] private TooltipWindow _tooltipWindow;

        private PoolMono<AbilityText> _pool;
        private List<AbilityText> _activeAbilities = new List<AbilityText>();

        private void Awake()
        {
            _pool = new(_prefab, _containerAbility, 5, true);
        }

        private void OnEnable()
        {
            ShowAbilites();
        }

        private void OnDisable()
        {
            UnsubscribeAbilityTextEvents();
            _tooltipWindow.gameObject.SetActive(false);
        }

        private void ShowAbilites()
        {
            UnsubscribeAbilityTextEvents();
            _activeAbilities.Clear();

            for (int i = 0; i < _containerAbility.childCount; i++)
            {
                var child = _containerAbility.GetChild(i);
                if (!child.TryGetComponent(out FightBackButton _))
                {
                    child.gameObject.SetActive(false);
                }
            }

            var fightService = NovelGame.Instance.GetService<FightService>();
            if (fightService.Player is null)
            {
                return;
            }
            var abilites = fightService.PlayerData.Abilities;

            foreach (var ability in abilites)
            {
                var abilityView = _pool.GetFreeElement();
                abilityView.SetAbility(ability);
                abilityView.gameObject.SetActive(true);

                abilityView.OnHover += OnAbilityHover;
                abilityView.OnExitHover += OnAbilityExitHover;
                abilityView.OnClickAbility += OnAbilityClick;
                abilityView.Interactable = true;
                _activeAbilities.Add(abilityView);

                if (ability.Cost > fightService.GetCurrentEnergyCharacter(fightService.PlayerData) || fightService.IsAbilityOnCooldown(fightService.PlayerData, ability))
                {
                    abilityView.OnClickAbility -= OnAbilityClick;
                    abilityView.Interactable = false;
                }
            }
        }

        private void OnAbilityClick(object sender, EventArgs eventArgs)
        {
            AbilityEventArgs abilityEventArgs = eventArgs as AbilityEventArgs;
            var ability = abilityEventArgs.Ability;
            var service = NovelGame.Instance.GetService<FightService>();
            var player = service.PlayerData;
            service.UseAbility(player, ability);
        }

        private void OnAbilityHover(ScriptableAbility ability)
        {
            _tooltipWindow.SetAbility(ability);
            _tooltipWindow.gameObject.SetActive(true);
        }

        private void OnAbilityExitHover(ScriptableAbility ability)
        {
            _tooltipWindow.gameObject.SetActive(false);
        }

        private void UnsubscribeAbilityTextEvents()
        {
            foreach (var abilityView in _activeAbilities)
            {
                if (abilityView != null)
                {
                    abilityView.OnHover -= OnAbilityHover;
                    abilityView.OnExitHover -= OnAbilityExitHover;
                    abilityView.OnClickAbility -= OnAbilityClick;
                }
            }
        }

    }
}