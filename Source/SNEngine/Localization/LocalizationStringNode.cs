using SiphoinUnityHelpers.XNodeExtensions.Variables;

namespace SNEngine.Localization
{
    public class LocalizationStringNode : StringNode, ILocalizationNode
    {
        public object GetOriginalValue()
        {
            return GetStartValue();
        }

        public object GetValue()
        {
            return GetStartValue();
        }

        public override void SetValue(object value)
        {
            if (value is string str)
            {
                base.SetValue(str);
            }
        }
    }
}
