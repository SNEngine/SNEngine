using System;
using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class FunctionEndInstructionHandler : BaseInstructionHandler
    {
        public override bool CanHandle(string instruction)
        {
            // Проверяем, является ли инструкция завершением функции (только lowercase "end")
            return string.Equals(instruction.Trim(), "end", StringComparison.Ordinal);
        }

        public override InstructionResult Handle(string instruction, InstructionContext context)
        {
            // Завершаем текущую функцию
            context.CurrentFunctionName = null;

            // Возвращаем успех
            return InstructionResult.Ok(new { Type = "FunctionEnd" });
        }
    }
}