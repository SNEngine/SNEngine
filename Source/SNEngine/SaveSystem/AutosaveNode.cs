using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;

namespace SNEngine.SaveSystem
{
    public class AutosaveNode : AsyncNode
    {
        private const string NAME_SAVE = "autosave";

        public override async void Execute()
        {
            base.Execute();
            var service = NovelGame.Instance.GetService<SaveLoadService>();
            await service.SaveCurrentState(NAME_SAVE);
            StopTask();
        }
    }
}
