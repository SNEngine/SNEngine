using UnityEngine;

namespace SNEngine.UI
{
    public interface IUIContainer
    {
        void AddComponent (RectTransform rectTransform, int? index = null);
    }
}
