namespace SNEngine.ConfirmationWindowSystem
{
    public readonly struct ConfirmationWindowResult
    {
        public ConfirmationWindowButtonType ButtonPressed { get; }

        public bool IsConfirmed => ButtonPressed.HasFlag(ConfirmationWindowButtonType.Yes) || 
                                  ButtonPressed.HasFlag(ConfirmationWindowButtonType.Ok);

        public ConfirmationWindowResult(ConfirmationWindowButtonType buttonPressed)
        {
            ButtonPressed = buttonPressed;
        }
    }
}