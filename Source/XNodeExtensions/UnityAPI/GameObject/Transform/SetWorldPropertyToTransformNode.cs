using UnityEngine;
using static XNode.Node;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent
{
    public abstract class SetWorldPropertyToTransformNode<TProperty> : SetDataToTransformNode<TProperty>
    {
        [SerializeField] private WorldType _type;

        protected override void SetData(Transform transform, TProperty data)
        {
            switch (_type)
            {
                case WorldType.Current:
                    SetCurrentProperty(transform, data); 
                    break;
                case WorldType.Local:
                    SetLocalProperty(transform, data);
                    break;
            }
        }

        protected abstract void SetCurrentProperty(Transform transform, TProperty data);

        protected abstract void SetLocalProperty(Transform transform, TProperty data);


    }
}
