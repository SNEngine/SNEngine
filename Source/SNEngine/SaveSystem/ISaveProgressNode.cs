namespace SNEngine.SaveSystem
{
    public interface ISaveProgressNode
    {
        string GUID { get; }
        object GetDataForSave();
        void SetDataFromSave(object data);
        void ResetSaveBehaviour();
    }
}
