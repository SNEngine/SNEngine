using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.TimeSystem
{
    public abstract class GetTimeValueNode : BaseNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private float _value;

        public override object GetValue(NodePort port)
        {
            return GetValue();
        }

        protected abstract float GetValue();
    }
}
