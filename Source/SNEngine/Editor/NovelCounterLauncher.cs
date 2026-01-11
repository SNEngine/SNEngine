namespace SNEngine.Editor
{
    public partial class NovelCounterWindow
    {
        private class NovelCounterLauncher : BaseToolLauncher
        {
            public static void Run(string args, System.Action<string> onLog) =>
                LaunchExecutable("SNEngine_Novel_Counter", "SNEngine_Novel_Counter.exe", "SNEngine_Novel_Counter", args, onLog);
        }
    }
}