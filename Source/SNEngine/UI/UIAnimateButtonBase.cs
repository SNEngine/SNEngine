using UnityEngine.EventSystems;
using UnityEngine;

namespace SNEngine.UI
{
    public abstract class UIAnimateButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public abstract void OnPointerEnter(PointerEventData eventData);

        public abstract void OnPointerExit(PointerEventData eventData);

        protected abstract void AnimateHover(bool isHovering);
    }
}