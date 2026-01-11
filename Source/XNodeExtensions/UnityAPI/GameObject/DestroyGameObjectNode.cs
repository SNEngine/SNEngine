using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects
{
    public class DestroyGameObjectNode : DestroyNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField, Min(0)] private float _time;

        protected override void DestroyGameObject(GameObject gameObject)
        {
            float time = _time;

            var inputTime = GetInputPort(nameof(_time));

            if (inputTime.Connection != null)
            {
                time = GetDataFromPort<float>(nameof(_time));
            }

            Destroy(gameObject, time);
        }
    }
}
