using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent
{
    public abstract class GetWorldPropertyFromTransformNode<TProperty> : GetDataFromTransformNode<TProperty>
    {
        [SerializeField] private WorldType _type;

        protected override TProperty GetData(Transform transform)
        {
            if (!Application.isPlaying)
            {
                return default;
            }

            switch (_type)
            {
                case WorldType.Current:
                    return GetCurrentProperty(transform);
                case WorldType.Local:
                    return GetLocalProperty(transform);
            }

            return default;


        }

        protected abstract TProperty GetCurrentProperty(Transform transform);

        protected abstract TProperty GetLocalProperty(Transform transform);
    }
}
