using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public abstract class BaseInstructionValidator : IInstructionValidator
    {
        public abstract bool CanValidate(string instruction);
        public abstract ValidationInstructionResult Validate(string instruction);

        protected (bool success, string value) ExtractValue(string instruction, string pattern)
        {
            var match = Regex.Match(instruction, pattern, RegexOptions.IgnoreCase);
            if (match.Success && match.Groups.Count > 1)
            {
                return (true, match.Groups[1].Value.Trim());
            }
            return (false, null);
        }
    }
}