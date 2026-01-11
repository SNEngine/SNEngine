using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Finding.ByName
{
    public abstract class FindWithNameNode<TResult> : BaseNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _name;

        [Output(ShowBackingValue.Never), SerializeField] private TResult _result;
        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return null;
            }

            string name = _name;

            var input = GetInputPort(nameof(_name));

            if (input.Connection != null)
            {
                name = GetDataFromPort<string>(nameof(_name));
            }

            return ReturnResult(name);


        }

        protected abstract TResult ReturnResult(string name);
    }
}
