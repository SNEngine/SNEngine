using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects
{
    public abstract class SetDataToGameObjectNode : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private GameObject _gameObject;
        public override void Execute()
        {
            var gameObject = GetDataFromPort<GameObject>(nameof(_gameObject));

            SetData(gameObject);
        }

        public abstract void SetData(GameObject gameObject);
    }
}
