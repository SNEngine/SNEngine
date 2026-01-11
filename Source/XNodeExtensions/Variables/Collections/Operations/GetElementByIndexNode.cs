using System.Collections;
using UnityEngine;
using XNode;
using System.Linq;
using System.Collections.Generic;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Collection.Operations
{
    public class GetElementByIndexNode : CollectionOperationNode<Object>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private int _index;

        public override Object Operation(List<object> list)
        {
            int index = _index;

            var indexInput = GetInputPort(nameof(_index));

            if (indexInput.Connection != null)
            {
                index = GetDataFromPort<int>(nameof(_index));
            }

            return list[index] as Object;
        }
    }
}
