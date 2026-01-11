using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects
{
    public abstract class GetDataFromGameObjectNode<TOutput> : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private GameObject _gameObject;

        [Output(ShowBackingValue.Never), SerializeField] private TOutput _output;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            var gameObject = GetDataFromPort<GameObject>(nameof(_gameObject));

            return GetData(gameObject);
        }
        protected abstract TOutput GetData(GameObject gameObject);
    }
}
