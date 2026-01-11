using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Finding.Tag
{
    public abstract class FindWithTagNode<TResult> : BaseNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _tag;

        [Output(ShowBackingValue.Never), SerializeField] private TResult _result;
        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return null;
            }

            string tag = _tag;

            var input = GetInputPort(nameof(_tag));

            if (input.Connection != null)
            {
                tag = GetDataFromPort<string>(nameof(_tag));
            }

            return ReturnResult(tag);


        }

        protected abstract TResult ReturnResult(string tag);
    }

}
