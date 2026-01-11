using SiphoinUnityHelpers.XNodeExtensions;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Random
{
    public abstract class RandomRangeNode<T> : BaseNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private T _min;

        [Input(connectionType = ConnectionType.Override), SerializeField] private T _max;


        [Output, SerializeField] private T _result;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            T min = Parse(_min.ToString());

            T max = Parse(_max.ToString());

            var inputMin = GetInputPort(nameof(_min));

            var inputMax = GetInputPort(nameof(_max));

            if (inputMin.Connection != null)
            {
                max = Parse(GetDataFromPort<T>(nameof(_min)).ToString());
            }

            if (inputMax.Connection != null)
            {
                max = Parse(GetDataFromPort<T>(nameof(_max)).ToString());
            }

            return Range(min, max);
        }

        protected abstract T Parse(string data);

        protected abstract T Range(T min, T max);
    }
}
