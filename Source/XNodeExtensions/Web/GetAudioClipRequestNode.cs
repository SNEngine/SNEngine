using SiphoinUnityHelpers.XNodeExtensions.Extensions;
using System;
using UnityEngine;
using UnityEngine.Networking;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Web
{
    public class GetAudioClipRequestNode : BaseWebRequestNode
    {
        [SerializeField] private AudioType _audioType = AudioType.MPEG;
        [Output, SerializeField] private AudioClip _audioClip;

        protected override UnityWebRequest CreateRequest(string targetUrl)
        {
            return UnityWebRequestMultimedia.GetAudioClip(targetUrl, _audioType);
        }

        protected override void OnRequestSuccess(UnityWebRequest request)
        {
            _audioClip = DownloadHandlerAudioClip.GetContent(request);
            _audioClip.name = $"download_audio_{Guid.NewGuid().ToShortGUID()}_{_audioClip.GetInstanceID()}";
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == nameof(_audioClip)) return _audioClip;
            return base.GetValue(port);
        }
    }
}