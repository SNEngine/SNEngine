namespace SNEngine.Editor
{
    public partial class CodeCounterWindow
    {
        private class CodeCounterLauncher : BaseToolLauncher
        {
            public static void Run(string args, System.Action<string> onLog) =>
                LaunchExecutable("SNEngine_Code_Counter", "SNEngine_Code_Counter.exe", "SNEngine_Code_Counter", args, onLog);
        }
    }
}