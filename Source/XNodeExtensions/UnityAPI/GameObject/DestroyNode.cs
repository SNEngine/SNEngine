

using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects
{
    public abstract class DestroyNode : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private GameObject _gameObject;

        public override void Execute()
        {
            var gameObject = GetDataFromPort<GameObject>(nameof(_gameObject));

            DestroyGameObject(gameObject);

        }

        protected abstract void DestroyGameObject(GameObject gameObject);

        
    }
}
