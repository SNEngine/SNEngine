using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.FunctionSystem
{
    public class SNILFunctionParser
    {
        public static List<SNILFunction> ParseFunctions(string[] lines)
        {
            List<SNILFunction> functions = new List<SNILFunction>();
            List<string> currentFunctionBody = null;
            string currentFunctionName = null;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (line.StartsWith("function ", StringComparison.OrdinalIgnoreCase))
                {
                    // Начинается новая функция
                    if (currentFunctionName != null)
                    {
                        // Завершаем предыдущую функцию
                        functions.Add(new SNILFunction
                        {
                            Name = currentFunctionName,
                            Body = currentFunctionBody?.ToArray() ?? new string[0]
                        });
                    }

                    // Извлекаем имя функции
                    string functionName = line.Substring(9).Trim(); // "function ".Length = 9
                    currentFunctionName = functionName;
                    currentFunctionBody = new List<string>();
                }
                else if (line.Equals("end", StringComparison.OrdinalIgnoreCase) && currentFunctionName != null)
                {
                    // Завершаем текущую функцию
                    functions.Add(new SNILFunction
                    {
                        Name = currentFunctionName,
                        Body = currentFunctionBody?.ToArray() ?? new string[0]
                    });
                    
                    currentFunctionName = null;
                    currentFunctionBody = null;
                }
                else if (currentFunctionName != null)
                {
                    // Добавляем строку к телу текущей функции
                    currentFunctionBody.Add(line);
                }
            }

            // Завершаем последнюю функцию, если она не была завершена
            if (currentFunctionName != null)
            {
                functions.Add(new SNILFunction
                {
                    Name = currentFunctionName,
                    Body = currentFunctionBody?.ToArray() ?? new string[0]
                });
            }

            return functions;
        }

        public static List<string> ExtractMainScriptWithoutFunctions(string[] lines)
        {
            List<string> mainScript = new List<string>();
            bool insideFunction = false;

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("function ", StringComparison.OrdinalIgnoreCase))
                {
                    insideFunction = true;
                }
                else if (trimmedLine.Equals("end", StringComparison.OrdinalIgnoreCase) && insideFunction)
                {
                    insideFunction = false;
                }
                else if (!insideFunction)
                {
                    mainScript.Add(line);
                }
            }

            return mainScript;
        }

        public static List<string> FindFunctionCalls(string[] lines)
        {
            List<string> functionCalls = new List<string>();
            
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                
                if (trimmedLine.StartsWith("call ", StringComparison.OrdinalIgnoreCase))
                {
                    string functionName = trimmedLine.Substring(5).Trim(); // "call ".Length = 5
                    functionCalls.Add(functionName);
                }
            }

            return functionCalls;
        }
    }

    public class SNILFunction
    {
        public string Name { get; set; }
        public string[] Body { get; set; }
    }
}