using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SiphoinUnityHelpers.XNodeExtensions.Math.Compare
{
    public class CompareObjectsNode : CompareNode
    {
        [SerializeField, Input(ShowBackingValue.Never, ConnectionType.Override)] private Object _a;

        [SerializeField, Input(ShowBackingValue.Never, ConnectionType.Override)] private Object _b;

        [SerializeField] private CompareObjectsType _type;

        protected override bool Compute()
        {
            var a = GetDataFromPort<object>(nameof(_a));

            var b = GetDataFromPort<object>(nameof(_b));

            if (a is null || b is null)
            {
                throw new NullReferenceException($"any argument on Compute Objects Node (GUID: {GUID}) as null ");
            }

            return CompareHelper.Compare(a, b, _type);
        }
    }
}
