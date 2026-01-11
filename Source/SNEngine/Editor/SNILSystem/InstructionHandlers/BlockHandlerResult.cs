using System.Collections.Generic;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class BlockHandlerResult
    {
        public List<SNILInstruction> Instructions { get; set; } = new List<SNILInstruction>();
        public List<(int RelativeInstructionIndex, string FunctionName)> FunctionCalls { get; set; } = new List<(int, string)>();
    }
}