using System.Collections.Generic;
using UnityEditor;
using SNEngine.Editor.SNILSystem;

namespace SNEngine.Editor.SNILSystem.Tests
{
    public class TestSetVariableInstruction
    {
        [MenuItem("SNEngine/Test Set Variable Instruction")]
        public static void TestSetVariable()
        {
            string testFilePath = "Assets/SNEngine/Source/test_set_variable_updated.snil";
            
            SNILDebug.Log("Starting test of Set Variable instruction...");
            
            // Проверяем валидацию
            if (SNILCompiler.ValidateScript(testFilePath, out List<Validators.SNILValidationError> validationErrors))
            {
                SNILDebug.Log("Validation passed!");
                
                // Пытаемся импортировать
                if (SNILCompiler.ImportScript(testFilePath))
                {
                    SNILDebug.Log("Import successful! Check the Dialogue Graphs in Resources/Dialogues.");
                }
                else
                {
                    SNILDebug.LogError("Import failed!");
                }
            }
            else
            {
                SNILDebug.LogError($"Validation failed with {validationErrors.Count} errors:");
                foreach (var error in validationErrors)
                {
                    SNILDebug.LogError($"  Line {error.LineNumber}: {error.Message} (Content: {error.LineContent})");
                }
            }
        }
    }
}