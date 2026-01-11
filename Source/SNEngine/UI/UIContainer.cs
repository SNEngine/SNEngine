using UnityEngine;
using System;
namespace SNEngine.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIContainer : MonoBehaviour, IUIContainer
    {
        public void AddComponent(RectTransform rectTransform, int? index = null)
        {
            if (rectTransform is null)
            {
                throw new ArgumentNullException("ui component is null");
            }

            rectTransform.SetParent(transform, false);

            if (index != null)
            {
                rectTransform.SetSiblingIndex(index.Value);
            }
        }
    }
}
