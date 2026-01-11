using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem
{
    public class SNILTemplateMatcher
    {
        public static Dictionary<string, string> MatchLineWithTemplate(string line, string template)
        {
            var paramMatches = Regex.Matches(template, @"[\{\[].*?[\}\]]");

            if (paramMatches.Count == 0)
            {
                return string.Equals(line, template, System.StringComparison.OrdinalIgnoreCase)
                    ? new Dictionary<string, string>()
                    : null;
            }

            string regexPattern = Regex.Escape(template);
            foreach (Match m in paramMatches)
            {
                string paramName = m.Value.Trim('{', '}', '[', ']');
                string escapedPlaceholder = Regex.Escape(m.Value);
                int index = regexPattern.IndexOf(escapedPlaceholder);

                if (index != -1)
                {
                    regexPattern = regexPattern.Remove(index, escapedPlaceholder.Length)
                                               .Insert(index, $"(?<{paramName}>.*?)");
                }
            }

            var matchResult = Regex.Match(line, "^" + regexPattern + "$");
            if (matchResult.Success)
            {
                var parameters = new Dictionary<string, string>();
                foreach (Match m in paramMatches)
                {
                    string name = m.Value.Trim('{', '}', '[', ']');
                    parameters[name] = matchResult.Groups[name].Value;
                }
                return parameters;
            }

            return null;
        }
    }
}