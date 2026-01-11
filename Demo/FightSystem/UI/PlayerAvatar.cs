using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using SNEngine.Extensions;
using UnityEngine;
using UnityEngine.UI;
namespace CoreGame.FightSystem.UI
{
    public class PlayerAvatar : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Image _image;

        public void SetAvatar (Sprite sprite)
        {
            if (!sprite)
            {
                Debug.LogError($"sprite for avatar is null");
                return;
            }

            _image.sprite = sprite;
            _image.SetAdaptiveSize();
        }

        private void OnValidate()
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
            }   
        }
    }
}
