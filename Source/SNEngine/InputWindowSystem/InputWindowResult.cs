namespace SNEngine.InputWindowSystem
{
    public struct InputWindowResult
    {
        public InputWindowResult(string input, InputWindowButton button)
        {
            Input = input;
            Button = button;
        }

        public string Input { get; set; }
        public InputWindowButton Button { get; set; }
    }
}