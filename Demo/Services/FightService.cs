using CoreGame.FightSystem;
using CoreGame.FightSystem.HealthSystem;
using CoreGame.FightSystem.Models;
using CoreGame.FightSystem.UI;
using SNEngine;
using SNEngine.CharacterSystem;
using SNEngine.Services;
using SNEngine.Utils;
using System.Collections.Generic;
using UnityEngine;
using System;
using CoreGame.FightSystem.AI;
using Object = UnityEngine.Object;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using SNEngine.Polling;
using CoreGame.FightSystem.Abilities;
using System.Linq;
using CoreGame.FightSystem.Utils;
using CoreGame.FightSystem.ManaSystem;

namespace CoreGame.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Service/New FightService")]
    public class FightService : ServiceBase
    {
        private CharacterService _characterService;
        private BackgroundService _backgroundService;
        private Dictionary<Character, IFightComponent> _fightComponents;
        private Dictionary<Character, float> _healthBeforeLastAction;
        private Dictionary<FightCharacter, List<AbilityEntity>> _currentAbilitesData;
        private Dictionary<FightCharacter, float> _currentEnergyData;
        private Dictionary<FightCharacter, List<AbilityEntity>> _activeTickEffects;
        private IFightWindow _fightWindow;
        private IFightComponent _player;
        private IFightComponent _enemy;
        private FightCharacter _playerCharacter;
        private FightCharacter _enemyCharacter;
        private AIFighter _aiFighter;
        private PoolMono<HealText> _poolHealText;
        private PoolMono<DamageText> _poolDamageText;
        private PoolMono<CriticalDamageText> _poolCriticalDamageText;
        private Dictionary<Character, bool> _wasLastHitCritical;
        private const string FIGHT_WINDOW_VANILLA_PATH = "UI/FightWindow";
        private const float ENEMY_TURN_DELAY = 0.5f;
        private FightTurnOwner _fightTurnOwner = FightTurnOwner.Player;
        private FightResult _result;
        [SerializeField] private float _hitShakeDuration = 0.3f;
        [SerializeField] private float _hitShakeStrength = 10f;
        [SerializeField] private int _hitShakeVibrato = 10;
        [SerializeField] private Color _hitColor = new Color(1f, 0f, 0f, 1f);
        [SerializeField] private float _hitColorDuration = 0.1f;
        [SerializeField] private Ease _hitColorEase = Ease.Linear;
        [SerializeField, Range(1, 10)] private int _defaultEnergyRestoreCooldown = 3;
        private Dictionary<FightCharacter, int> _energyRestoreCounters;
        private const float ENERGY_RESTORE_AMOUNT = 1f;
        private Dictionary<string, ScriptableAbility> _allAbilitiesByGUID;

        public IFightComponent Player => _player;
        public IFightComponent Enemy => _enemy;
        public FightCharacter PlayerData { get; private set; }
        public FightCharacter EnemyData { get; private set; }
        public event Action<FightResult> OnFightEnded;
        public event Action<FightCharacter, ScriptableAbility, float> OnAbilityUsed;
        public event Action<FightCharacter, float> OnPlayerHealed;
        public event Action<FightCharacter, FightCharacter, float> OnCriticalHit;

        public override void Initialize()
        {
            _characterService = NovelGame.Instance.GetService<CharacterService>();
            _backgroundService = NovelGame.Instance.GetService<BackgroundService>();
            var ui = NovelGame.Instance.GetService<UIService>();
            var input = ResourceLoader.LoadCustomOrVanilla<FightWindow>(FIGHT_WINDOW_VANILLA_PATH);
            var healTextPrefab = ResourceLoader.LoadCustomOrVanilla<HealText>("UI/HealText");
            var damageTextPrefab = ResourceLoader.LoadCustomOrVanilla<DamageText>("UI/DamageText");
            var criticalDamageTextPrefab = ResourceLoader.LoadCustomOrVanilla<CriticalDamageText>("UI/CriticalDamageText");
            var containerTexts = new GameObject("Floating Texts");
            containerTexts.AddComponent<RectTransform>();
            if (input == null) return;
            var prefab = Object.Instantiate(input);
            prefab.name = input.name;
            _fightWindow = prefab;
            ui.AddElementToUIContainer(prefab.gameObject);
            prefab.transform.SetAsFirstSibling();

            prefab.gameObject.SetActive(false);
            containerTexts.transform.SetParent(prefab.transform, false);
            _poolHealText = new PoolMono<HealText>(healTextPrefab, containerTexts.transform, 9, true);
            _poolDamageText = new PoolMono<DamageText>(damageTextPrefab, containerTexts.transform, 9, true);
            _poolCriticalDamageText = new PoolMono<CriticalDamageText>(criticalDamageTextPrefab, containerTexts.transform, 9, true);
        }

        public override void ResetState()
        {
            if (_fightWindow != null) _fightWindow.OnTurnExecuted -= OnPlayerTurnExecuted;
            UnsubscribeFromHealthEvents();
            HideCharacters();
            ClearupFightComponents();
            if (_fightWindow != null) _fightWindow.ResetState();
            _aiFighter = null;
            _player = null;
            _enemy = null;
            PlayerData = null;
            EnemyData = null;
            _healthBeforeLastAction = null;
            _wasLastHitCritical = null;
            _currentEnergyData = null;
            _currentAbilitesData = null;
            _energyRestoreCounters = null;
            _activeTickEffects = null;
        }

        public FightServiceSaveData GetSaveData()
        {
            if (_playerCharacter == null || _enemyCharacter == null || _player == null || _enemy == null)
            {
                return null;
            }

            var saveData = new FightServiceSaveData
            {
                PlayerCharacterGUID = _playerCharacter.ReferenceCharacter.GUID,
                EnemyCharacterGUID = _enemyCharacter.ReferenceCharacter.GUID,
                CurrentTurnOwner = (int)_fightTurnOwner,
                PlayerEnergyRestoreCounter = _energyRestoreCounters.GetValueOrDefault(_playerCharacter),
                EnemyEnergyRestoreCounter = _energyRestoreCounters.GetValueOrDefault(_enemyCharacter),
                IsPlayerGuarding = _player.IsGuarding,
                IsEnemyGuarding = _enemy.IsGuarding,
                PlayerData = GetCharacterSaveData(_playerCharacter, _player),
                EnemyData = GetCharacterSaveData(_enemyCharacter, _enemy),
                Result = _result,
            };

            return saveData;
        }

        private FightCharacterSaveData GetCharacterSaveData(FightCharacter fightCharacter, IFightComponent fightComponent)
        {
            var characterData = new FightCharacterSaveData
            {
                CurrentHealth = fightComponent.HealthComponent.CurrentHealth,
                CurrentEnergy = _currentEnergyData.GetValueOrDefault(fightCharacter),
                AbilitiesData = new List<AbilitySaveData>(),
                ActiveTickEffects = new List<AbilitySaveData>()
            };

            var abilities = _currentAbilitesData.GetValueOrDefault(fightCharacter);
            if (abilities != null)
            {
                characterData.AbilitiesData.AddRange(
                    abilities.Select(e => new AbilitySaveData
                    {
                        AbilityGUID = e.ReferenceAbility.GUID,
                        CurrentCooldown = e.CurrentCooldown,
                        RemainingTicks = 0
                    })
                );
            }

            var activeTicks = _activeTickEffects.GetValueOrDefault(fightCharacter);
            if (activeTicks != null)
            {
                characterData.ActiveTickEffects.AddRange(
                    activeTicks.Select(e => new AbilitySaveData
                    {
                        AbilityGUID = e.ReferenceAbility.GUID,
                        CurrentCooldown = 0,
                        RemainingTicks = e.RemainingTicks
                    })
                );
            }

            return characterData;
        }

        private void PreloadAllAbilities()
        {
            if (_allAbilitiesByGUID != null) return;

            _allAbilitiesByGUID = new Dictionary<string, ScriptableAbility>();

            ScriptableAbility[] allAbilities = Resources.LoadAll<ScriptableAbility>("Skills");

            foreach (var ability in allAbilities)
            {
                if (ability != null && !string.IsNullOrEmpty(ability.GUID) && !_allAbilitiesByGUID.ContainsKey(ability.GUID))
                {
                    _allAbilitiesByGUID.Add(ability.GUID, ability);
                }
            }
        }

        public void TurnFight(FightCharacter playerCharacter, FightCharacter enemyCharacter, FightServiceSaveData saveData = null)
        {
            _playerCharacter = playerCharacter;
            _enemyCharacter = enemyCharacter;
            _fightComponents = new Dictionary<Character, IFightComponent>();
            _healthBeforeLastAction = new Dictionary<Character, float>();
            _wasLastHitCritical = new Dictionary<Character, bool>
            {
                { playerCharacter.ReferenceCharacter, false },
                { enemyCharacter.ReferenceCharacter, false }
            };
            _currentAbilitesData = new Dictionary<FightCharacter, List<AbilityEntity>>();
            _currentEnergyData = new Dictionary<FightCharacter, float>();
            _energyRestoreCounters = new Dictionary<FightCharacter, int>();
            _activeTickEffects = new Dictionary<FightCharacter, List<AbilityEntity>>();

            PreloadAllAbilities();

            SetupAbilitesCharacterForFight(playerCharacter);
            SetupAbilitesCharacterForFight(enemyCharacter);

            SetupCharacterForFight(playerCharacter);
            SetupCharacterForFight(enemyCharacter);

            _player = _fightComponents[playerCharacter.ReferenceCharacter];
            _enemy = _fightComponents[enemyCharacter.ReferenceCharacter];
            EnemyData = enemyCharacter;
            PlayerData = playerCharacter;

            ScriptableAI enemyAI = _enemyCharacter.ReferenceAI;
            _aiFighter = new AIFighter(_enemyCharacter, _enemy, _player, enemyAI);

            _fightWindow.SetData(_player, _enemy, playerCharacter, enemyCharacter);
            _fightWindow.OnTurnExecuted += OnPlayerTurnExecuted;

            if (saveData != null)
            {
                LoadFightState(saveData);
                SubscribeToHealthEvents();
                InitializeHealthActionTracking();
                _fightWindow.UpdateHealthAndEnergyDisplay(_player, _enemy, playerCharacter, enemyCharacter);
            }
            else
            {
                SubscribeToHealthEvents();
                InitializeHealthActionTracking();
                StartNewTurn(FightTurnOwner.Player);
            }
            _fightWindow.Show();

            _result = FightResult.None;
        }

        private void InitializeHealthActionTracking()
        {
            if (_player != null && _playerCharacter != null)
            {
                _healthBeforeLastAction[_playerCharacter.ReferenceCharacter] = _player.HealthComponent.CurrentHealth;
            }
            if (_enemy != null && _enemyCharacter != null)
            {
                _healthBeforeLastAction[_enemyCharacter.ReferenceCharacter] = _enemy.HealthComponent.CurrentHealth;
            }
        }

        private void LoadFightState(FightServiceSaveData saveData)
        {
            LoadCharacterState(_playerCharacter, _player, saveData.PlayerData, saveData.PlayerEnergyRestoreCounter);
            LoadCharacterState(_enemyCharacter, _enemy, saveData.EnemyData, saveData.EnemyEnergyRestoreCounter);

            if (saveData.IsPlayerGuarding) _player.StartGuard();
            if (saveData.IsEnemyGuarding) _enemy.StartGuard();

            _fightTurnOwner = (FightTurnOwner)saveData.CurrentTurnOwner;

            StartNewTurn(_fightTurnOwner, isInitialLoad: true);
        }

        private void LoadCharacterState(FightCharacter fightCharacter, IFightComponent fightComponent, FightCharacterSaveData data, int energyRestoreCounter)
        {
            fightComponent.HealthComponent.SetData(fightCharacter.Health);

            _currentEnergyData[fightCharacter] = data.CurrentEnergy;
            _energyRestoreCounters[fightCharacter] = energyRestoreCounter;

            float initialHealth = fightCharacter.Health;
            float loadedHealth = data.CurrentHealth;
            float delta = loadedHealth - initialHealth;

            if (Mathf.Abs(delta) > 0.01f)
            {
                if (delta < 0)
                {
                    fightComponent.HealthComponent.TakeDamage(Mathf.Abs(delta));
                }
                else
                {
                    fightComponent.HealthComponent.Heal(delta);
                }
            }

            _currentAbilitesData[fightCharacter].Clear();
            foreach (var ability in fightCharacter.Abilities)
            {
                var entity = new AbilityEntity(ability);
                var savedData = data.AbilitiesData.FirstOrDefault(d => d.AbilityGUID == entity.ReferenceAbility.GUID);

                if (savedData != null)
                {
                    entity.CurrentCooldown = savedData.CurrentCooldown;
                }

                _currentAbilitesData[fightCharacter].Add(entity);
            }

            _activeTickEffects[fightCharacter].Clear();
            foreach (var savedTickData in data.ActiveTickEffects)
            {
                ScriptableAbility abilityAsset = LoadAbilityByGUID(savedTickData.AbilityGUID);
                if (abilityAsset is ScriptableOverTurnAbility overTurnAbility)
                {
                    var tickEntity = new AbilityEntity(overTurnAbility);
                    tickEntity.RemainingTicks = savedTickData.RemainingTicks;
                    _activeTickEffects[fightCharacter].Add(tickEntity);
                }
            }
        }

        private ScriptableAbility LoadAbilityByGUID(string guid)
        {
            if (_allAbilitiesByGUID == null) return null;

            if (_allAbilitiesByGUID.TryGetValue(guid, out var ability))
            {
                return ability;
            }

            return null;
        }

        private void SetupCharacterForFight(FightCharacter character)
        {
            _characterService.ShowCharacter(character.ReferenceCharacter);
            ICharacterRenderer characterRenderer = _characterService.GetWorldCharacter(character.ReferenceCharacter);
            IFightComponent fightComponent = characterRenderer.AddComponent<FightComponent>();
            fightComponent.AddComponents();
            fightComponent.HealthComponent.SetData(character.Health);
            fightComponent.ManaComponent.SetData(character.EnergyPoint);
            fightComponent.SetFightCharacter(character);
            _fightComponents.Add(character.ReferenceCharacter, fightComponent);
        }

        private void SetupAbilitesCharacterForFight(FightCharacter fightCharacter)
        {
            _currentEnergyData.Add(fightCharacter, fightCharacter.EnergyPoint);
            _energyRestoreCounters.Add(fightCharacter, _defaultEnergyRestoreCooldown);
            _currentAbilitesData.Add(fightCharacter, new List<AbilityEntity>());
            _activeTickEffects.Add(fightCharacter, new List<AbilityEntity>());
            foreach (var ability in fightCharacter.Abilities)
                _currentAbilitesData[fightCharacter].Add(new AbilityEntity(ability));
        }

        private void ClearupFightComponents()
        {
            if (_fightComponents != null)
            {
                foreach (var component in _fightComponents.Values)
                    if (component is FightComponent fightComponent)
                    {
                        HealthComponent healthComponent = fightComponent.gameObject.GetComponent<HealthComponent>();
                        ManaComponent manaComponent = fightComponent.gameObject.GetComponent<ManaComponent>();
                        Object.Destroy(fightComponent);
                        Object.Destroy(manaComponent);
                        Object.Destroy(healthComponent);
                    }
                _fightComponents.Clear();
            }
        }

        private void HideCharacters()
        {
            if (_fightComponents != null)
                foreach (var character in _fightComponents.Keys)
                    _characterService.HideCharacter(character);
        }

        private void ProgressCooldowns()
        {
            var characters = new List<FightCharacter> { PlayerData, EnemyData };
            foreach (var fightCharacter in characters)
            {
                if (_energyRestoreCounters.ContainsKey(fightCharacter))
                {
                    if (_energyRestoreCounters[fightCharacter] > 0) _energyRestoreCounters[fightCharacter]--;
                    if (_energyRestoreCounters[fightCharacter] == 0)
                    {
                        float currentEnergy = _currentEnergyData.GetValueOrDefault(fightCharacter);
                        float maxEnergy = fightCharacter.EnergyPoint;
                        if (currentEnergy < maxEnergy)
                        {
                            _currentEnergyData[fightCharacter] = Mathf.Min(currentEnergy + ENERGY_RESTORE_AMOUNT, maxEnergy);
                            OnAbilityUsed?.Invoke(fightCharacter, null, _currentEnergyData[fightCharacter]);
                            _energyRestoreCounters[fightCharacter] = _defaultEnergyRestoreCooldown;
                        }
                    }
                }
            }
            foreach (var abilityList in _currentAbilitesData.Values)
                foreach (var abilityEntity in abilityList)
                    if (abilityEntity.CurrentCooldown > 0) abilityEntity.CurrentCooldown--;
        }

        private void ProcessAbilitiesOverTurns()
        {
            IEnumerable<FightCharacter> characters = _activeTickEffects.Keys.ToList();

            foreach (var character in characters)
            {
                IFightComponent userComponent = _fightComponents.GetValueOrDefault(character.ReferenceCharacter);
                if (userComponent == null) continue;

                IFightComponent targetComponent = (userComponent == _player) ? _enemy : _player;

                var effectsToProcess = _activeTickEffects[character].ToList();

                for (int i = effectsToProcess.Count - 1; i >= 0; i--)
                {
                    var entity = effectsToProcess[i];

                    if (entity.ReferenceAbility is ScriptableOverTurnAbility overTurnAbility)
                    {
                        if (entity.RemainingTicks > 0)
                        {
                            overTurnAbility.ExecuteTurnTick(userComponent, targetComponent);

                            entity.RemainingTicks--;

                            if (entity.RemainingTicks <= 0)
                            {
                                _activeTickEffects[character].RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }

        private void StartNewTurn(FightTurnOwner newOwner, bool isInitialLoad = false)
        {
            _fightTurnOwner = newOwner;

            if (_fightTurnOwner == FightTurnOwner.Player)
                _player.StopGuard();
            else
                _enemy.StopGuard();

            if (!isInitialLoad)
            {
                ProcessAbilitiesOverTurns();
                ProgressCooldowns();
            }

            if (_fightTurnOwner == FightTurnOwner.Player)
                _fightWindow.ShowPanelAction();
            else
                ExecuteEnemyTurn().Forget();
        }

        private async void OnPlayerTurnExecuted(PlayerAction action)
        {
            if (_fightTurnOwner != FightTurnOwner.Player) return;
            _fightWindow.HidePanelAction();
            SaveHealthBeforeAction();
            await HandlePlayerAction(action);
            if (CheckFightEndConditions()) return;
            StartNewTurn(FightTurnOwner.Enemy);
        }

        private async UniTask HandlePlayerAction(PlayerAction action)
        {
            switch (action)
            {
                case PlayerAction.Attack:
                    await HandleAttackAction(_enemy, _playerCharacter.Damage, _enemyCharacter, _playerCharacter);
                    break;
                case PlayerAction.Guard:
                    _player.StartGuard();
                    break;
            }
        }

        private async UniTask ExecuteEnemyTurn()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(ENEMY_TURN_DELAY), DelayType.DeltaTime, PlayerLoopTiming.Update, CancellationToken.None);
            SaveHealthBeforeAction();
            IReadOnlyList<AbilityEntity> enemyAbilities = _currentAbilitesData.GetValueOrDefault(_enemyCharacter);
            float enemyEnergy = _currentEnergyData.GetValueOrDefault(_enemyCharacter);
            AIDecision enemyDecision = _aiFighter.DecideAction(enemyAbilities, enemyEnergy);
            await HandleEnemyAction(enemyDecision);
            if (CheckFightEndConditions()) return;
            await UniTask.Delay(TimeSpan.FromSeconds(ENEMY_TURN_DELAY), DelayType.DeltaTime, PlayerLoopTiming.Update, CancellationToken.None);
            StartNewTurn(FightTurnOwner.Player);
        }

        private async UniTask HandleEnemyAction(AIDecision decision)
        {
            switch (decision.Action)
            {
                case PlayerAction.Attack:
                    await HandleAttackAction(_player, _enemyCharacter.Damage, _playerCharacter, _enemyCharacter);
                    break;
                case PlayerAction.Guard:
                    _enemy.StartGuard();
                    break;
                case PlayerAction.Wait:
                    break;
                case PlayerAction.UseSkill:
                    if (decision.Ability != null) await UseAbility(_enemyCharacter, decision.Ability);
                    break;
            }
        }

        private async UniTask HandleAttackAction(IFightComponent targetComponent, float baseDamage, FightCharacter targetCharacter, FightCharacter attackerCharacter)
        {
            float finalDamage = baseDamage;
            bool isCritical = false;

            if (UnityEngine.Random.Range(0f, 1f) < attackerCharacter.CriticalHitChance)
            {
                finalDamage *= attackerCharacter.CriticalHitMultiplier;
                isCritical = true;
            }

            if (targetComponent.IsGuarding)
            {
                finalDamage = DamageUtils.ApplyGuardReduction(targetCharacter, finalDamage);
                targetComponent.StopGuard();
            }

            _wasLastHitCritical[targetCharacter.ReferenceCharacter] = isCritical;
            targetComponent.HealthComponent.TakeDamage(finalDamage);

            if (isCritical)
            {
                OnCriticalHit?.Invoke(attackerCharacter, targetCharacter, finalDamage);
            }
        }

        public async UniTask UseAbility(FightCharacter fightCharacter, ScriptableAbility ability)
        {
            if (fightCharacter != _playerCharacter && fightCharacter != _enemyCharacter) return;

            var currentEnergyCharacter = _currentEnergyData.GetValueOrDefault(fightCharacter, 0f);
            var abilityList = _currentAbilitesData.GetValueOrDefault(fightCharacter);
            var abilityEntity = abilityList.FirstOrDefault(e => e.ReferenceAbility == ability);

            if (abilityEntity != null && currentEnergyCharacter >= ability.Cost)
            {
                _currentEnergyData[fightCharacter] = currentEnergyCharacter - ability.Cost;
                abilityEntity.CurrentCooldown = ability.Cooldown;
                _energyRestoreCounters[fightCharacter] = _defaultEnergyRestoreCooldown;

                IFightComponent userComponent = fightCharacter == _playerCharacter ? _player : _enemy;
                IFightComponent targetComponent = fightCharacter == _playerCharacter ? _enemy : _player;

                ability.ExecuteEffect(userComponent, targetComponent);

                if (ability is ScriptableOverTurnAbility overTurnAbility)
                {
                    var tickEntity = new AbilityEntity(overTurnAbility);

                    if (!_activeTickEffects.ContainsKey(fightCharacter))
                    {
                        _activeTickEffects[fightCharacter] = new List<AbilityEntity>();
                    }

                    tickEntity.RemainingTicks = overTurnAbility.Duration;

                    _activeTickEffects[fightCharacter].Add(tickEntity);
                }

                OnAbilityUsed?.Invoke(fightCharacter, ability, _currentEnergyData[fightCharacter]);

                await HandleAbilityUsage(fightCharacter, ability);
            }
        }

        public float GetCurrentEnergyCharacter(FightCharacter fightCharacter)
        {
            return _currentEnergyData.GetValueOrDefault(fightCharacter);
        }

        public bool IsAbilityOnCooldown(FightCharacter fightCharacter, ScriptableAbility ability)
        {
            var abilites = _currentAbilitesData[fightCharacter];
            var entity = abilites.First(x => x.ReferenceAbility == ability);
            return entity.CurrentCooldown > 0;
        }

        private async UniTask HandleAbilityUsage(FightCharacter fightCharacter, ScriptableAbility ability)
        {
            _fightWindow.HidePanelSkills();
            _fightWindow.HidePanelAction();
            await UniTask.Delay(TimeSpan.FromSeconds(ability.Duration), DelayType.DeltaTime, PlayerLoopTiming.Update, CancellationToken.None);

            if (CheckFightEndConditions()) return;

            if (fightCharacter == _playerCharacter && _fightTurnOwner == FightTurnOwner.Player)
                StartNewTurn(FightTurnOwner.Enemy);
            else if (fightCharacter == _enemyCharacter && _fightTurnOwner == FightTurnOwner.Enemy)
                StartNewTurn(FightTurnOwner.Player);
        }

        private void SaveHealthBeforeAction()
        {
            if (_playerCharacter != null && _player != null)
            {
                _healthBeforeLastAction[_playerCharacter.ReferenceCharacter] = _player.HealthComponent.CurrentHealth;
            }
            if (_enemyCharacter != null && _enemy != null)
            {
                _healthBeforeLastAction[_enemyCharacter.ReferenceCharacter] = _enemy.HealthComponent.CurrentHealth;
            }
        }

        private void SubscribeToHealthEvents()
        {
            if (_player != null) _player.HealthComponent.OnHealthChanged += OnPlayerHealthChanged;
            if (_enemy != null) _enemy.HealthComponent.OnHealthChanged += OnEnemyHealthChanged;
        }

        private void UnsubscribeFromHealthEvents()
        {
            if (_player?.HealthComponent != null) _player.HealthComponent.OnHealthChanged -= OnPlayerHealthChanged;
            if (_enemy?.HealthComponent != null) _enemy.HealthComponent.OnHealthChanged -= OnEnemyHealthChanged;
        }

        private void OnPlayerHealthChanged(float currentHealth, float maxHealth) => HandleHealthChange(_playerCharacter.ReferenceCharacter, currentHealth);
        private void OnEnemyHealthChanged(float currentHealth, float maxHealth) => HandleHealthChange(_enemyCharacter.ReferenceCharacter, currentHealth);

        private async void HandleHealthChange(Character character, float currentHealth)
        {
            float healthBefore = _healthBeforeLastAction.GetValueOrDefault(character, currentHealth);
            float delta = currentHealth - healthBefore;

            if (Mathf.Abs(delta) < 0.01f)
            {
                _healthBeforeLastAction[character] = currentHealth;
                return;
            }

            Vector3 worldPosition = _characterService.GetCharacterWorldPosition(character);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            if (delta > 0)
            {
                _poolHealText.GetFreeElement().Show(delta, screenPosition).Forget();
                if (character == _playerCharacter.ReferenceCharacter) OnPlayerHealed?.Invoke(_playerCharacter, delta);
            }
            else
            {
                float damage = Mathf.Abs(delta);
                if (_wasLastHitCritical.GetValueOrDefault(character, false))
                {
                    _poolCriticalDamageText.GetFreeElement().Show(damage, screenPosition).Forget();
                    _wasLastHitCritical[character] = false;
                }
                else _poolDamageText.GetFreeElement().Show(damage, screenPosition).Forget();

                if (character == _playerCharacter.ReferenceCharacter && currentHealth > 0)
                {
                    await HandleBackgroundHitAnimation();
                }
                else if (character == _enemyCharacter.ReferenceCharacter && currentHealth > 0)
                {
                    await HandleHitAnimation(character);
                }
            }

            _healthBeforeLastAction[character] = currentHealth;
        }

        private async UniTask HandleHitAnimation(Character character)
        {
            await UniTask.WhenAll(
                _characterService.ShakePosition(character, _hitShakeDuration, _hitShakeStrength, _hitShakeVibrato, true),
                AnimateCharacterHit(character)
            );
        }

        private async UniTask HandleBackgroundHitAnimation()
        {
            await UniTask.WhenAll(
                _backgroundService.ShakePosition(_hitShakeDuration, _hitShakeStrength, _hitShakeVibrato, true),
                AnimateBackgroundHit()
            );
        }

        private async UniTask AnimateCharacterHit(Character character)
        {
            await _characterService.SetColorCharacter(character, _hitColor, _hitColorDuration, _hitColorEase);
            await _characterService.SetColorCharacter(character, Color.white, _hitColorDuration, _hitColorEase);
        }

        private async UniTask AnimateBackgroundHit()
        {
            await _backgroundService.SetColor(_hitColor, _hitColorDuration, _hitColorEase);
            await _backgroundService.SetColor(Color.white, _hitColorDuration, _hitColorEase);
        }

        private bool CheckFightEndConditions()
        {
            bool playerDead = _player.HealthComponent.CurrentHealth <= 0;
            bool enemyDead = _enemy.HealthComponent.CurrentHealth <= 0;

            if (playerDead && enemyDead) { EndFight(FightResult.Tie); return true; }
            if (playerDead) { EndFight(FightResult.Defeat); return true; }
            if (enemyDead) { EndFight(FightResult.Victory); return true; }
            return false;
        }

        private void EndFight(FightResult result)
        {
            _fightWindow.OnTurnExecuted -= OnPlayerTurnExecuted;
            UnsubscribeFromHealthEvents();
            _fightWindow.Hide();
            HideCharacters();
            ClearupFightComponents();
            _result = result;
            OnFightEnded?.Invoke(result);
        }
    }
}
