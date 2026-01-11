using System;
using System.Collections.Generic;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public interface IBlockInstructionHandler
    {
        /// <summary>
        /// Handles a multi-line block instruction starting at currentLineIndex.
        /// Should update currentLineIndex to the last consumed line index.
        /// Returns an InstructionResult whose Data is a List<SNILInstruction> on success.
        /// </summary>
        InstructionResult HandleBlock(string[] lines, ref int currentLineIndex, InstructionContext context);
    }
}