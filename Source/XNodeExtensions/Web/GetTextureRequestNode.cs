using SiphoinUnityHelpers.XNodeExtensions.Extensions;
using System;
using UnityEngine;
using UnityEngine.Networking;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Web
{
    public class GetTextureRequestNode : BaseWebRequestNode
    {
        [Output, SerializeField] private Texture2D _texture;

        protected override UnityWebRequest CreateRequest(string targetUrl)
        {
            return UnityWebRequestTexture.GetTexture(targetUrl);
        }

        protected override void OnRequestSuccess(UnityWebRequest request)
        {
            _texture = DownloadHandlerTexture.GetContent(request);
            _texture.name = $"download_texture_{Guid.NewGuid().ToShortGUID()}_{_texture.GetInstanceID()}";
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == nameof(_texture)) return _texture;
            return base.GetValue(port);
        }
    }
}