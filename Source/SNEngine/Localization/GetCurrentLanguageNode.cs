using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Services;
using UnityEngine;
using XNode;

namespace SNEngine.Localization
{
    public class GetCurrentLanguageNode : BaseNodeInteraction
    {
        [Output(ShowBackingValue.Never), SerializeField] private string _code;

        public override object GetValue(NodePort port)
        {
            return NovelGame.Instance.GetService<LanguageService>().CurrentLanguageCode;
        }
    }
}
