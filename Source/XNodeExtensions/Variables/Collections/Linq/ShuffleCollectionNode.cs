using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Collection.Linq
{
    public class ShuffleCollectionNode : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private NodePortEnumerable _enumerable;
        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            System.Random random = new System.Random();

            var enumerable = GetDataFromPort<IEnumerable>(nameof(_enumerable));

            var list = new List<object>();

            foreach ( var item in enumerable)
            {
                list.Add(item);
            }

            return list.OrderBy(x => random.Next());
        }
    }
}
