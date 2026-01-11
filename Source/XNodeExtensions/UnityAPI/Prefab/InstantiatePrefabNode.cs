using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.Prefabs
{
    public class InstantiatePrefabNode : BaseNodeInteraction
    {
        [SerializeField, Input] private GameObject _prefab;

        [SerializeField, Input(ShowBackingValue.Never, ConnectionType.Override)] private GameObject _parent;

        [SerializeField, Output(ShowBackingValue.Never)] private GameObject _output;

        private GameObject _result;

        public override void Execute()
        {
            Transform targetTransform = null;

            var valueParent = GetDataFromPort<GameObject>(nameof(_parent));

            var selectedPrefab = _prefab;

            var prefabFromConnection = GetDataFromPort<GameObject>(nameof(_prefab));

            if (prefabFromConnection != null)
            {
                selectedPrefab = prefabFromConnection;
            }

            if (valueParent != null)
            {
                targetTransform = valueParent.transform;
            }

            _result = Instantiate(selectedPrefab, targetTransform);
        }

        public override object GetValue(NodePort port)
        {
            return _result;
        }
    }
}
