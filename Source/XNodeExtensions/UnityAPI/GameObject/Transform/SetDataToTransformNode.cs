using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent
{
    public abstract class SetDataToTransformNode<TData> : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private Transform _transform;

        [Input(connectionType = ConnectionType.Override), SerializeField] private TData _data;
        public override void Execute()
        {
            TData data = _data;

            var transform = GetDataFromPort<Transform>(nameof(_transform));

            var inputData = GetInputPort(nameof(_data));

            if (inputData.Connection != null)
            {
                data = GetInputValue<TData>(nameof(_data));
            }

            SetData(transform, data);
            
        }
        protected abstract void SetData(Transform transform, TData data);
    }
}
