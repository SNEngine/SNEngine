using UnityEngine;

namespace SNEngine
{
    public class NovelCamera : MonoBehaviour
    {
        private Camera _camera;

        private const float TargetAspect = 1.777778f; // 16:9
        private const float DefaultSize = 5.4f;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            AdjustCamera();
        }

        private void OnGUI()
        {
            AdjustCamera();
        }

        private void AdjustCamera()
        {
            float currentAspect = (float)Screen.width / Screen.height;

            if (currentAspect < TargetAspect)
            {
                float calculatedSize = DefaultSize * (TargetAspect / currentAspect);
                _camera.orthographicSize = Mathf.Round(calculatedSize * 10f) / 10f;
            }
            else
            {
                _camera.orthographicSize = DefaultSize;
            }
        }
    }
}