using UnityEngine;
using UnityEngine.Networking;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Web
{
    public class GetTextRequestNode : BaseWebRequestNode
    {
        [Output, SerializeField] private string _text;

        protected override UnityWebRequest CreateRequest(string targetUrl)
        {
            return UnityWebRequest.Get(targetUrl);
        }

        protected override void OnRequestSuccess(UnityWebRequest request)
        {
            _text = request.downloadHandler.text;
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == nameof(_text)) return _text;
            return base.GetValue(port);
        }
    }
}