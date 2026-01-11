using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.Child
{
    public class GetChildByIndexNode : GetDataFromTransformNode<GameObject>
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private int _index;

        protected override GameObject GetData(Transform transform)
        {
            int index = _index;

           var input = GetInputPort(nameof(_index));

            if (input.Connection != null)
            {
                index = GetDataFromPort<int>(nameof(_index));
            }

            return transform.GetChild(index).gameObject;
        }
    }
}
