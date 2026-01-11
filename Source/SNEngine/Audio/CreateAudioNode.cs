using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Services;
using UnityEngine;
using XNode;

namespace SNEngine.Audio
{
    public class CreateAudioNode : AudioNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private AudioObject _result;

        public override void Execute()
        {
            var service = NovelGame.Instance.GetService<AudioService>();
            _result = service.GetFreeAudioObject() as AudioObject;
        }

        public override object GetValue(NodePort port)
        {
            return _result;
        }
    }
}
