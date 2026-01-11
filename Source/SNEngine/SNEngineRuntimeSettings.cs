using UnityEngine;
using DG.Tweening;

namespace SNEngine
{
    [CreateAssetMenu(fileName = "SNEngineRuntimeSettings", menuName = "SNEngine/Runtime Settings")]
    public class SNEngineRuntimeSettings : ScriptableObject
    {
        public bool ShowVideoSplash = true;
        public bool EnableCrossfade = true;
        public float CrossfadeDuration = 0.3f;
        public Ease CrossfadeEase = Ease.Linear;

        private static SNEngineRuntimeSettings _instance;

        public static SNEngineRuntimeSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<SNEngineRuntimeSettings>("SNEngineRuntimeSettings");

                    if (_instance == null)
                    {
                        _instance = ScriptableObject.CreateInstance<SNEngineRuntimeSettings>();
                    }
                }
                return _instance;
            }
        }
    }
}