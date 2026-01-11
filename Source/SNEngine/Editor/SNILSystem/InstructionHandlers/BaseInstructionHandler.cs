using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public abstract class BaseInstructionHandler : IInstructionHandler
    {
        public abstract bool CanHandle(string instruction);
        public abstract InstructionResult Handle(string instruction, InstructionContext context);

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