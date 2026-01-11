using System.Collections.Generic;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public abstract class SNILValidator
    {
        public abstract bool Validate(string[] lines, out string errorMessage);
    }
}