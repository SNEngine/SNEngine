using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.ConfirmationWindowSystem;
using SNEngine.SaveSystem.Models;
using SNEngine.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SNEngine.SaveSystem.UI
{
    public class SaveView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Animation Settings")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _fadeOutDuration = 0.3f;
        [SerializeField] private Ease _animationEase = Ease.OutQuad;

        [Header("UI Components")]
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private TextMeshProUGUI _textNameSave;
        [SerializeField] private TextMeshProUGUI _textDateSave;
        [SerializeField] private Button _button;
        [SerializeField] private Button _deleteButton;

        private string _saveName;
        private CanvasGroup _deleteButtonCanvasGroup;
        private Tween _currentTween;

        public event Action<string> OnSelect;

        private void OnEnable()
        {
            _button.onClick.AddListener(Select);

            if (_deleteButton != null)
            {
                // Initialize canvas group for smooth alpha transitions
                _deleteButtonCanvasGroup = _deleteButton.GetComponent<CanvasGroup>();
                if (_deleteButtonCanvasGroup == null)
                {
                    _deleteButtonCanvasGroup = _deleteButton.gameObject.AddComponent<CanvasGroup>();
                }

                // Initially hide the delete button
                _deleteButtonCanvasGroup.alpha = 0f;
                _deleteButtonCanvasGroup.interactable = false;
                _deleteButtonCanvasGroup.blocksRaycasts = false;

                _deleteButton.onClick.AddListener(DeleteSave);
            }
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Select);

            if (_deleteButton != null)
            {
                _deleteButton.onClick.RemoveListener(DeleteSave);
            }

            // Kill any active tweens
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
            }
        }

        private void Select()
        {
            OnSelect?.Invoke(_saveName);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_deleteButton != null)
            {
                // Kill any active tweens before starting a new one
                if (_currentTween != null && _currentTween.IsActive())
                {
                    _currentTween.Kill();
                }

                // Fade in the delete button
                _currentTween = _deleteButtonCanvasGroup.DOFade(1f, _fadeInDuration)
                    .SetEase(_animationEase)
                    .OnComplete(() => {
                        _deleteButtonCanvasGroup.interactable = true;
                        _deleteButtonCanvasGroup.blocksRaycasts = true;
                    });
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_deleteButton != null)
            {
                // Kill any active tweens before starting a new one
                if (_currentTween != null && _currentTween.IsActive())
                {
                    _currentTween.Kill();
                }

                // Fade out the delete button
                _currentTween = _deleteButtonCanvasGroup.DOFade(0f, _fadeOutDuration)
                    .SetEase(_animationEase)
                    .OnComplete(() => {
                        _deleteButtonCanvasGroup.interactable = false;
                        _deleteButtonCanvasGroup.blocksRaycasts = false;
                    });
            }
        }

        private async void DeleteSave()
        {
            // Temporarily disable the delete button during confirmation
            if (_deleteButtonCanvasGroup != null)
            {
                _deleteButtonCanvasGroup.interactable = false;
                _deleteButtonCanvasGroup.blocksRaycasts = false;
            }

            var confirmationService = NovelGame.Instance.GetService<ConfirmationWindowService>();

            confirmationService.SetData(
                "delete_save",
                "confirm_delete_save_message",
                null,
                ConfirmationWindowButtonType.YesNo,
                "Delete Save",
                string.Format("Are you sure you want to delete '{0}'?", _saveName)
            );
            confirmationService.Show();

            var result = await confirmationService.WaitForConfirmation();
            confirmationService.Hide();

            // Re-enable the delete button after confirmation
            if (_deleteButtonCanvasGroup != null)
            {
                _deleteButtonCanvasGroup.interactable = true;
                _deleteButtonCanvasGroup.blocksRaycasts = true;
            }

            if (result.IsConfirmed)
            {
                var saveLoadService = NovelGame.Instance.GetService<SaveLoadService>();
                bool success = await saveLoadService.DeleteSave(_saveName);

                if (success)
                {
                    // Notify the parent to refresh the list
                    var parentListView = GetComponentInParent<SaveListView>();
                    if (parentListView != null)
                    {
                        parentListView.RefreshList();
                    }
                }
            }
        }

        public void SetData (PreloadSave data)
        {
            _rawImage.texture = data.PreviewTexture;
            _saveName = data.SaveName;
            _textNameSave.text = data.SaveName;
            _textDateSave.text = data.SaveData.DateSave.ToString("dd.mm:yyyy");
        }
    }
}