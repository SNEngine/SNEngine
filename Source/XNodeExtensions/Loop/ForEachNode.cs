using System.Collections;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Loop
{
    public class ForEachNode : LoopNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private NodePortEnumerable _enumerable;

        [Space(10)]

        [Output(ShowBackingValue.Never), SerializeField] private Object _outputElement;

        private object _currentElement;

        public override object GetValue(NodePort port)
        {
            return _currentElement;
        }

        public override void Execute()
        {
            var enumerable = GetDataFromPort<IEnumerable>(nameof(_enumerable));

            if (enumerable == null) return;

            foreach (var item in enumerable)
            {
                _currentElement = item;

                CallLoop();
            }
        }
    }
}