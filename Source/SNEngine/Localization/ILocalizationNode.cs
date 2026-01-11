namespace SNEngine.Localization
{
    public interface ILocalizationNode
    {
        string GUID { get; }
        object GetOriginalValue();
        void SetValue(object value);
        object GetValue();
    }
}
