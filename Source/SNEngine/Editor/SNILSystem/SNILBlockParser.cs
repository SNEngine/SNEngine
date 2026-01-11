using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem
{
    [System.Obsolete("SNILBlockParser is deprecated: block parsing is now handled by instruction handlers. This class remains only for compatibility and will throw when used.")]
    public class SNILBlockParser
    {
        public static List<SNILInstruction> ParseWithBlocks(string[] lines)
        {
            throw new NotSupportedException("SNILBlockParser has been removed. Use the instruction handler based parser (SNILScriptProcessor.ParseLinesToInstructions) instead.");
        }

        private static bool IsIfStatement(string line)
        {
            return line.StartsWith("IF", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsBlockControlStatement(string line)
        {
            string lowerLine = line.ToLower();
            return lowerLine == "elif" || lowerLine == "else" || lowerLine == "endif";
        }

        private static bool IsComment(string line)
        {
            return line.StartsWith("//") || line.StartsWith("#");
        }

        private static List<SNILInstruction> ParseIfBlock(string[] lines, ref int currentIndex, Dictionary<string, SNILTemplateInfo> templates)
        {
            List<SNILInstruction> blockInstructions = new List<SNILInstruction>();

            // Parse the initial IF condition
            string ifLine = lines[currentIndex].Trim();
            string condition = ExtractCondition(ifLine);

            // Create CompareIntegers node for the condition (simplified - in real implementation,
            // this would parse the actual comparison like "selectedIndex == 0")
            // For now, we'll create a simple structure that represents the IF logic

            // Add a CompareIntegers node to evaluate the condition
            var compareInstruction = CreateCompareInstruction(condition, templates);
            if (compareInstruction != null)
            {
                blockInstructions.Add(compareInstruction);
            }

            // Create IfNode that will use the comparison result
            var ifInstruction = CreateIfInstruction(templates);
            if (ifInstruction != null)
            {
                blockInstructions.Add(ifInstruction);
            }

            // Parse the IF branch content
            currentIndex++;
            int ifBranchEnd = FindNextControlStatement(lines, currentIndex, new[] { "ELIF", "ELSE", "ENDIF" });

            // Parse content between IF and next control statement
            var ifBranchInstructions = ParseInstructionsInRange(lines, currentIndex, ifBranchEnd - 1, templates);
            blockInstructions.AddRange(ifBranchInstructions);

            // Handle ELIF and ELSE branches
            while (currentIndex < ifBranchEnd && currentIndex < lines.Length)
            {
                string controlLine = lines[currentIndex].Trim();

                if (controlLine.Equals("ELIF", StringComparison.OrdinalIgnoreCase))
                {
                    // Handle ELIF branch - this would require more complex logic for multiple conditions
                    // For now, we'll just skip to the next control statement
                    currentIndex++;
                    int elifBranchEnd = FindNextControlStatement(lines, currentIndex, new[] { "ELIF", "ELSE", "ENDIF" });
                    var elifBranchInstructions = ParseInstructionsInRange(lines, currentIndex, elifBranchEnd - 1, templates);
                    blockInstructions.AddRange(elifBranchInstructions);
                    currentIndex = elifBranchEnd - 1; // Adjust for the loop increment
                }
                else if (controlLine.Equals("ELSE", StringComparison.OrdinalIgnoreCase))
                {
                    // Handle ELSE branch
                    currentIndex++;
                    int elseBranchEnd = FindNextControlStatement(lines, currentIndex, new[] { "ENDIF" });
                    var elseBranchInstructions = ParseInstructionsInRange(lines, currentIndex, elseBranchEnd - 1, templates);
                    blockInstructions.AddRange(elseBranchInstructions);
                    currentIndex = elseBranchEnd - 1; // Adjust for the loop increment
                }
                else if (controlLine.Equals("ENDIF", StringComparison.OrdinalIgnoreCase))
                {
                    // End of IF block
                    break;
                }

                currentIndex++;
            }

            return blockInstructions;
        }

        private static string ExtractCondition(string ifLine)
        {
            // Extract condition from "IF condition" format
            ifLine = ifLine.Trim();
            if (ifLine.StartsWith("IF", StringComparison.OrdinalIgnoreCase))
            {
                return ifLine.Substring(2).Trim();
            }
            return "";
        }

        private static SNILInstruction CreateCompareInstruction(string condition, Dictionary<string, SNILTemplateInfo> templates)
        {
            // This is a simplified version - in reality, you'd parse the condition properly
            // For example: "selectedIndex == 0" would become a CompareIntegersNode
            var template = templates.FirstOrDefault(t => t.Key.Equals("CompareIntegersNode", StringComparison.OrdinalIgnoreCase)).Value;
            if (template != null)
            {
                // Parse the condition to extract a, operator, b
                var parameters = ParseComparison(condition);
                
                return new SNILInstruction
                {
                    Type = SNILInstructionType.Generic,
                    NodeTypeName = "CompareIntegersNode",
                    Parameters = parameters,
                    NodeType = SNILTypeResolver.GetNodeType("CompareIntegersNode")
                };
            }
            
            return null;
        }

        private static Dictionary<string, string> ParseComparison(string condition)
        {
            var parameters = new Dictionary<string, string>();

            // Simple parsing for "a operator b" format
            string[] parts = condition.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 3)
            {
                parameters["a"] = parts[0]; // e.g., "selectedIndex"
                parameters["type"] = MapOperator(parts[1]); // e.g., "==" becomes "Equals"
                parameters["b"] = parts[2]; // e.g., "0"
            }
            else
            {
                // Handle more complex parsing if needed
                // For now, try to parse using regex for "value operator value" pattern
                var match = Regex.Match(condition.Trim(), @"(\S+)\s+(==|!=|>|<|>=|<=)\s+(\S+)");
                if (match.Success)
                {
                    parameters["a"] = match.Groups[1].Value;
                    parameters["type"] = MapOperator(match.Groups[2].Value);
                    parameters["b"] = match.Groups[3].Value;
                }
            }

            return parameters;
        }

        private static string MapOperator(string op)
        {
            switch (op)
            {
                case "==": return "Equals";
                case "!=": return "NotEquals";
                case ">": return "More";
                case "<": return "Lesser";
                case ">=": return "MoreOrEquals";
                case "<=": return "LesserOrEquals";
                default: return "Equals"; // default fallback
            }
        }

        private static SNILInstruction CreateIfInstruction(Dictionary<string, SNILTemplateInfo> templates)
        {
            var template = templates.FirstOrDefault(t => t.Key.Equals("IfNode", StringComparison.OrdinalIgnoreCase)).Value;
            if (template != null)
            {
                return new SNILInstruction
                {
                    Type = SNILInstructionType.Generic,
                    NodeTypeName = "IfNode",
                    Parameters = new Dictionary<string, string>(),
                    NodeType = SNILTypeResolver.GetNodeType("IfNode")
                };
            }
            
            return null;
        }

        private static List<SNILInstruction> ParseInstructionsInRange(string[] lines, int start, int end, Dictionary<string, SNILTemplateInfo> templates)
        {
            List<SNILInstruction> instructions = new List<SNILInstruction>();
            
            for (int i = start; i <= end; i++)
            {
                string line = lines[i].Trim();
                
                if (string.IsNullOrEmpty(line) || IsComment(line))
                {
                    continue;
                }

                // Check if this is a control statement that should not be processed here
                if (IsBlockControlStatement(line))
                {
                    continue; // These are handled by the parent block parser
                }

                var instruction = MatchLineToTemplate(line, templates);
                if (instruction != null)
                {
                    instructions.Add(instruction);
                }
            }
            
            return instructions;
        }

        private static int FindNextControlStatement(string[] lines, int startIndex, string[] controlStatements)
        {
            for (int i = startIndex; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line) || IsComment(line))
                    continue;
                    
                foreach (string control in controlStatements)
                {
                    if (line.Equals(control, StringComparison.OrdinalIgnoreCase))
                    {
                        return i;
                    }
                }
            }
            return lines.Length; // If no control statement found, return end of array
        }

        private static SNILInstruction MatchLineToTemplate(string line, Dictionary<string, SNILTemplateInfo> templates)
        {
            foreach (var template in templates)
            {
                var parameters = SNILTemplateMatcher.MatchLineWithTemplate(line, template.Value.Template);
                if (parameters != null)
                {
                    return new SNILInstruction
                    {
                        Type = SNILInstructionType.Generic,
                        NodeTypeName = template.Key,
                        Parameters = parameters,
                        NodeType = SNILTypeResolver.GetNodeType(template.Key)
                    };
                }
            }

            return null;
        }
    }
}