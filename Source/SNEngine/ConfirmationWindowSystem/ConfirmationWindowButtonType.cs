using System;

namespace SNEngine.ConfirmationWindowSystem
{
    [Flags]
    public enum ConfirmationWindowButtonType
    {
        None = 0,
        Ok = 1,
        Cancel = 2,
        Yes = 4,
        No = 8,
        OkCancel = Ok | Cancel,
        YesNo = Yes | No
    }
}