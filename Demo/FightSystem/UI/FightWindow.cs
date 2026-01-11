using CoreGame.FightSystem;
using CoreGame.FightSystem.Abilities;
using CoreGame.Services;
using CoreGame.Utilities;
using DG.Tweening;
using SNEngine;
using SNEngine.Polling;
using SNEngine.Services;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = CoreGame.Utilities.ColorUtility;

namespace CoreGame.FightSystem.UI
{
    public class FightWindow : MonoBehaviour, IFightWindow
    {
        private const string USES_LOCALIZE_KEY = "fight_uses_prefix";
        private const string ABILITY_DESC_KEY_PREFIX = "ability_";
        private const string ABILITY_DESC_KEY_SUFFIX = "_name";
        private const string CRITICAL_DAMAGE_KEY_MESSAGE = "fight_critical_damage_message";
        private const string DAMAGE_KEY_MESSAGE = "fight_damage";
        private const string FROM_DAMAGE_KEY_MESSAGE = "fight_from";
        [Header("UI Components")]
        [SerializeField] private FillSlider _healthPlayer;
        [SerializeField] private FillSlider _healthEnemy;
        [SerializeField] private ClickableText _attackButton;
        [SerializeField] private ClickableText _waitButton;
        [SerializeField] private ClickableText _guardButton;
        [SerializeField] private ClickableText _skillButton;
        [SerializeField] private RectTransform _panelAction;
        [SerializeField] private EnergyFill _energyFillPrefab;
        [SerializeField] private RectTransform _playerEnergyContainer;
        [SerializeField] private RectTransform _enemyEnergyContainer;
        [SerializeField] private RectTransform _panelListActions;
        [SerializeField] private AbilityWindow _abilityWindow;
        [SerializeField] private HintUI _hintUI;

        [Header("Avatars")]
        [SerializeField] private PlayerAvatar _playerAvatar;
        [SerializeField] private PlayerAvatar _enemyAvatar;

        [Header("Health Bar Color Components")]
        [SerializeField] private Image _healthPlayerFillImage;
        [SerializeField] private Image _healthEnemyFillImage;

        [Header("Health Bar Animation Settings")]
        [SerializeField, Min(0)] private float _durationChangeHealth = 0.3f;
        [SerializeField] private Ease _easeHealthAnimation = Ease.InBounce;

        [Header("Heal Overlay Settings")]
        [SerializeField] private Image _healOverlayImage;
        [SerializeField, Range(0, 1)] private float _healOverlayAlpha = 0.3f;
        [SerializeField, Min(0)] private float _healFadeDuration = 0.5f;

        [Header("UI Show/Hide Settings")]
        public const float ANIMATION_DURATION_HIDE = 0.5f;
        [SerializeField] private Ease _easeShowAnimation = Ease.OutBack;
        [SerializeField] private float _panelActionOffsetY = -500f;
        [SerializeField] private float _healthBarOffsetY = 300f;
        [SerializeField] private float _avatarOffsetY = 500f;

        public event Action<PlayerAction> OnTurnExecuted;

        private Vector2 _initialPanelActionPosition;
        private Vector2 _initialHealthPlayerPosition;
        private Vector2 _initialHealthEnemyPosition;
        private Vector2 _initialPlayerEnergyPosition;
        private Vector2 _initialEnemyEnergyPosition;
        private Vector2 _initialPlayerAvatarPosition;
        private Vector2 _initialEnemyAvatarPosition;

        private RectTransform _playerHealthParentRT;
        private RectTransform _enemyHealthParentRT;
        private RectTransform _playerAvatarRT;
        private RectTransform _enemyAvatarRT;

        private PoolMono<EnergyFill> _poolEnergyFillsPlayer;
        private PoolMono<EnergyFill> _poolEnergyFillsEnemy;

        private FightCharacter _playerData;
        private FightCharacter _enemyData;

        private FightService _fightService;

        private void Awake()
        {
            _playerHealthParentRT = _healthPlayer.transform.parent.GetComponent<RectTransform>();
            _enemyHealthParentRT = _healthEnemy.transform.parent.GetComponent<RectTransform>();

            _playerAvatarRT = _playerAvatar.transform.parent.GetComponent<RectTransform>();
            _enemyAvatarRT = _enemyAvatar.transform.parent.GetComponent<RectTransform>();

            _initialPanelActionPosition = _panelAction.anchoredPosition;
            _initialHealthPlayerPosition = _playerHealthParentRT.anchoredPosition;
            _initialHealthEnemyPosition = _enemyHealthParentRT.anchoredPosition;
            _initialPlayerEnergyPosition = _playerEnergyContainer.anchoredPosition;
            _initialEnemyEnergyPosition = _enemyEnergyContainer.anchoredPosition;
            _initialPlayerAvatarPosition = _playerAvatarRT.anchoredPosition;
            _initialEnemyAvatarPosition = _enemyAvatarRT.anchoredPosition;

            _attackButton.AddListener(() => OnClickButtonAction(PlayerAction.Attack));
            _guardButton.AddListener(() => OnClickButtonAction(PlayerAction.Guard));
            _waitButton.AddListener(() => OnClickButtonAction(PlayerAction.Wait));
            _skillButton.AddListener(OnClickSkillButton);

            CreatePoolEnergyFills(ref _poolEnergyFillsPlayer, _playerEnergyContainer);
            CreatePoolEnergyFills(ref _poolEnergyFillsEnemy, _enemyEnergyContainer);

            if (_healOverlayImage != null)
            {
                var color = _healOverlayImage.color;
                color.a = 0f;
                _healOverlayImage.color = color;
            }
        }

        private void CreatePoolEnergyFills(ref PoolMono<EnergyFill> poolEnergyFills, RectTransform container)
        {
            poolEnergyFills = new PoolMono<EnergyFill>(_energyFillPrefab, container, 10, true);
        }

        private void ShowEnergyPoints(PoolMono<EnergyFill> pool, FightCharacter fightCharacter, float currentEnergy)
        {
            int maxEnergy = fightCharacter.EnergyPoint;
            int currentFillCount = Mathf.CeilToInt(currentEnergy);

            for (int i = 0; i < maxEnergy; i++)
            {
                var fill = pool.Objects.ElementAt(i);
                fill.gameObject.SetActive(true);

                if (i >= maxEnergy - currentFillCount)
                    fill.SetStateFull();
                else
                    fill.SetStateEmpty();
            }
        }

        private void UpdateEnergyAfterAbility(FightCharacter user, ScriptableAbility ability, float currentEnergy)
        {
            bool isPlayer = user == _playerData;
            if (isPlayer)
                ShowEnergyPoints(_poolEnergyFillsPlayer, _playerData, currentEnergy);
            else
                ShowEnergyPoints(_poolEnergyFillsEnemy, _enemyData, currentEnergy);
        }

        private void OnClickButtonAction(PlayerAction action)
        {
            OnTurnExecuted?.Invoke(action);
        }

        private void OnClickSkillButton()
        {
            _panelListActions.gameObject.SetActive(false);
            _abilityWindow.gameObject.SetActive(true);
        }

        public void ResetState()
        {
            if (_fightService != null)
            {
                _fightService.OnAbilityUsed -= UpdateEnergyAfterAbility;
                _fightService.OnPlayerHealed -= HandlePlayerHealed;
                _fightService.OnAbilityUsed -= HandleAbilityUsed;
                _fightService.OnCriticalHit -= OnCriticalHit;
            }
        }

        private Tweener AnimateUIElement(RectTransform rectTransform, Vector2 endPosition)
        {
            rectTransform.DOKill();
            return rectTransform.DOAnchorPos(endPosition, ANIMATION_DURATION_HIDE).SetEase(_easeShowAnimation);
        }

        public void Show()
        {
            _fightService = NovelGame.Instance.GetService<FightService>();
            _fightService.OnAbilityUsed += UpdateEnergyAfterAbility;
            _fightService.OnPlayerHealed += HandlePlayerHealed;
            _fightService.OnAbilityUsed += HandleAbilityUsed;
            _fightService.OnCriticalHit += OnCriticalHit;

            _panelAction.gameObject.SetActive(true);
            _abilityWindow.gameObject.SetActive(false);
            _panelListActions.gameObject.SetActive(true);
            gameObject.SetActive(true);

            Sequence showSequence = DOTween.Sequence();
            showSequence.SetLink(gameObject);

            float playerHideOffsetY = -_healthBarOffsetY;
            float playerAvatarHideOffsetY = -_avatarOffsetY;

            _panelAction.anchoredPosition = _initialPanelActionPosition + new Vector2(0, _panelActionOffsetY);
            _playerHealthParentRT.anchoredPosition = _initialHealthPlayerPosition + new Vector2(0, playerHideOffsetY);
            _enemyHealthParentRT.anchoredPosition = _initialHealthEnemyPosition + new Vector2(0, _healthBarOffsetY);
            _playerEnergyContainer.anchoredPosition = _initialPlayerEnergyPosition + new Vector2(0, playerHideOffsetY);
            _enemyEnergyContainer.anchoredPosition = _initialEnemyEnergyPosition + new Vector2(0, _healthBarOffsetY);
            _playerAvatarRT.anchoredPosition = _initialPlayerAvatarPosition + new Vector2(0, playerAvatarHideOffsetY);
            _enemyAvatarRT.anchoredPosition = _initialEnemyAvatarPosition + new Vector2(0, _avatarOffsetY);

            showSequence.Append(AnimateUIElement(_panelAction, _initialPanelActionPosition));
            showSequence.Join(AnimateUIElement(_playerHealthParentRT, _initialHealthPlayerPosition));
            showSequence.Join(AnimateUIElement(_enemyHealthParentRT, _initialHealthEnemyPosition));
            showSequence.Join(AnimateUIElement(_playerEnergyContainer, _initialPlayerEnergyPosition));
            showSequence.Join(AnimateUIElement(_enemyEnergyContainer, _initialEnemyEnergyPosition));
            showSequence.Join(AnimateUIElement(_playerAvatarRT, _initialPlayerAvatarPosition));
            showSequence.Join(AnimateUIElement(_enemyAvatarRT, _initialEnemyAvatarPosition));
        }



        public void Hide()
        {
            if (_fightService != null)
            {
                _fightService.OnAbilityUsed -= UpdateEnergyAfterAbility;
                _fightService.OnPlayerHealed -= HandlePlayerHealed;
                _fightService.OnAbilityUsed -= HandleAbilityUsed;
                _fightService.OnCriticalHit -= OnCriticalHit;
            }

            Sequence hideSequence = DOTween.Sequence();
            hideSequence.SetLink(gameObject);

            float playerHideOffsetY = -_healthBarOffsetY;
            float playerAvatarHideOffsetY = -_avatarOffsetY;

            hideSequence.Append(AnimateUIElement(_panelAction, _initialPanelActionPosition + new Vector2(0, _panelActionOffsetY)));
            hideSequence.Join(AnimateUIElement(_playerHealthParentRT, _initialHealthPlayerPosition + new Vector2(0, playerHideOffsetY)));
            hideSequence.Join(AnimateUIElement(_enemyHealthParentRT, _initialHealthEnemyPosition + new Vector2(0, _healthBarOffsetY)));
            hideSequence.Join(AnimateUIElement(_playerEnergyContainer, _initialPlayerEnergyPosition + new Vector2(0, playerHideOffsetY)));
            hideSequence.Join(AnimateUIElement(_enemyEnergyContainer, _initialEnemyEnergyPosition + new Vector2(0, _healthBarOffsetY)));
            hideSequence.Join(AnimateUIElement(_playerAvatarRT, _initialPlayerAvatarPosition + new Vector2(0, playerAvatarHideOffsetY)));
            hideSequence.Join(AnimateUIElement(_enemyAvatarRT, _initialEnemyAvatarPosition + new Vector2(0, _avatarOffsetY)));


            hideSequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

        public void ShowPanelAction() => _panelAction.gameObject.SetActive(true);
        public void HidePanelAction() => _panelAction.gameObject.SetActive(false);
        public void HidePanelSkills()
        {
            _abilityWindow.gameObject.SetActive(false);
            _panelListActions.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            _panelAction.DOKill();
            _playerHealthParentRT.DOKill();
            _enemyHealthParentRT.DOKill();
            _playerEnergyContainer.DOKill();
            _enemyEnergyContainer.DOKill();
            _playerAvatarRT.DOKill();
            _enemyAvatarRT.DOKill();
        }

        public void SetData(IFightComponent fightComponentPlayer, IFightComponent fightComponentEnemy, FightCharacter playerData, FightCharacter enemyData)
        {
            _playerData = playerData;
            _enemyData = enemyData;

            fightComponentEnemy.HealthComponent.OnHealthChanged += OnHealthChangedEnemy;
            fightComponentPlayer.HealthComponent.OnHealthChanged += OnHealthChangedPlayer;

            _healthEnemy.MaxValue = fightComponentEnemy.HealthComponent.MaxHealth;
            _healthPlayer.MaxValue = fightComponentPlayer.HealthComponent.MaxHealth;

            if (_playerAvatar != null)
                _playerAvatar.SetAvatar(playerData.Avatar);
            if (_enemyAvatar != null)
                _enemyAvatar.SetAvatar(enemyData.Avatar);

            Color playerColor = playerData.Color;
            Color playerDarkenedColor = ColorUtility.Darken(playerColor, 0.25f);

            if (_playerHealthParentRT.TryGetComponent(out Image playerBackground))
                playerBackground.color = playerDarkenedColor;

            if (_healthPlayerFillImage != null)
                _healthPlayerFillImage.color = playerColor;

            Color enemyColor = enemyData.Color;
            Color enemyDarkenedColor = ColorUtility.Darken(enemyColor, 0.25f);

            if (_enemyHealthParentRT.TryGetComponent(out Image enemyBackground))
                enemyBackground.color = enemyDarkenedColor;

            if (_healthEnemyFillImage != null)
                _healthEnemyFillImage.color = enemyColor;

            UpdateHealthAndEnergyDisplay(fightComponentPlayer, fightComponentEnemy, playerData, enemyData);
        }

        public void UpdateHealthAndEnergyDisplay(IFightComponent fightComponentPlayer, IFightComponent fightComponentEnemy, FightCharacter playerData, FightCharacter enemyData)
        {
            _healthEnemy.SetValueSmoothly(fightComponentEnemy.HealthComponent.CurrentHealth, _durationChangeHealth, _easeHealthAnimation);
            _healthPlayer.SetValueSmoothly(fightComponentPlayer.HealthComponent.CurrentHealth, _durationChangeHealth, _easeHealthAnimation);

            FightService fightService = NovelGame.Instance.GetService<FightService>();
            float currentEnergyPlayer = fightService.GetCurrentEnergyCharacter(playerData);
            float currentEnergyEnemy = fightService.GetCurrentEnergyCharacter(enemyData);

            ShowEnergyPoints(_poolEnergyFillsEnemy, enemyData, currentEnergyEnemy);
            ShowEnergyPoints(_poolEnergyFillsPlayer, playerData, currentEnergyPlayer);
        }

        private void OnHealthChangedPlayer(float current, float max)
        {
            _healthPlayer.SetValueSmoothly(current, _durationChangeHealth, _easeHealthAnimation);
        }

        private void OnHealthChangedEnemy(float current, float max)
        {
            _healthEnemy.SetValueSmoothly(current, _durationChangeHealth, _easeHealthAnimation);
        }

        private void HandlePlayerHealed(FightCharacter player, float amount)
        {
            _healOverlayImage.DOKill();
            _healOverlayImage.color = new Color(
                _healOverlayImage.color.r,
                _healOverlayImage.color.g,
                _healOverlayImage.color.b,
                0f
            );

            _healOverlayImage.DOFade(_healOverlayAlpha, _healFadeDuration / 2).OnComplete(() =>
            {
                _healOverlayImage.DOFade(0f, _healFadeDuration / 2);
            });
        }

        private void HandleAbilityUsed(FightCharacter fightCharacter, ScriptableAbility ability, float currentEnergy)
        {
            if (ability != null && _hintUI != null)
            {
                var languageService = NovelGame.Instance.GetService<LanguageService>();
                string usesPrefix = languageService.LanguageIsLoaded ? languageService.TransliteUI(USES_LOCALIZE_KEY) : "uses";
                string abilityLocalizeKey = $"{ABILITY_DESC_KEY_PREFIX}{ability.GUID}{ABILITY_DESC_KEY_SUFFIX}";
                string abilityName = languageService.LanguageIsLoaded ? languageService.TransliteUI(abilityLocalizeKey) : ability.NameAbility;
                string characterName = fightCharacter.ReferenceCharacter.GetName();
                string hintMessage = $"{characterName} {usesPrefix}: {abilityName}";
                _hintUI.ShowHint(hintMessage);
            }
        }

        private void OnCriticalHit(FightCharacter attacker, FightCharacter hitTarget, float damwge)
        {
            var languageService = NovelGame.Instance.GetService<LanguageService>();
            string nameAttacker = attacker.ReferenceCharacter.GetName();
            string nameTarget = hitTarget.ReferenceCharacter.GetName();
            float damageOut = Mathf.Round(damwge);
            string criticalDamagePrefix = languageService.LanguageIsLoaded ? languageService.TransliteUI(CRITICAL_DAMAGE_KEY_MESSAGE) : "takes critical";
            string damagePrefix = languageService.LanguageIsLoaded ? languageService.TransliteUI(DAMAGE_KEY_MESSAGE) : "damage";
            string damageFromPrefix = languageService.LanguageIsLoaded ? languageService.TransliteUI(FROM_DAMAGE_KEY_MESSAGE) : "from";
            string message = $"{nameTarget} {criticalDamagePrefix} {damageOut} {damagePrefix} {damageFromPrefix} {nameAttacker}";
            _hintUI.ShowHint(message);

        }
    }
}