using System.Collections.Generic;
using static XNode.Node;
using UnityEngine;
using XNode;
using System.Collections;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Collection
{
    public abstract class CollectionOperationNode<TOutput> : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private NodePortEnumerable _enumerable;


        [Output(ShowBackingValue.Never), SerializeField] private TOutput _output;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            var enumerable = GetDataFromPort<IEnumerable>(nameof(_enumerable));

            var list = new List<object>();

            foreach (var item in enumerable)
            {
                list.Add(item);
            }

            return Operation(list);

        }
        public abstract TOutput Operation(List<object> list);
    }
}
