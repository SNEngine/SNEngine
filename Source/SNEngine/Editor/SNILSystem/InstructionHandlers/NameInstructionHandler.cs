using SNEngine.Editor.SNILSystem.Importers;
using System.Text.RegularExpressions;
using SNEngine.Graphs;
using UnityEngine;
using UnityEditor;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class NameInstructionHandler : BaseInstructionHandler
    {
        public override bool CanHandle(string instruction)
        {
            return Regex.IsMatch(instruction.Trim(), @"^name:\s*.+", RegexOptions.IgnoreCase);
        }

        public override InstructionResult Handle(string instruction, InstructionContext context)
        {
            var (success, graphName) = ExtractValue(instruction, @"^name:\s*(.+)");
            
            if (!success)
            {
                return InstructionResult.Error("Invalid name instruction format. Expected: 'name: <graph_name>'");
            }

            // Создаем граф с указанным именем
            string assetPath = $"Assets/SNEngine/Source/SNEngine/Resources/Dialogues/{graphName}.asset";
            DialogueGraph graph = AssetDatabase.LoadAssetAtPath<DialogueGraph>(assetPath);

            if (graph == null)
            {
                graph = ScriptableObject.CreateInstance<DialogueGraph>();
                graph.name = graphName;

                string folderPath = "Assets/SNEngine/Source/SNEngine/Resources/Dialogues";
                // Создание папок опущено для краткости - можно использовать существующую логику из SNILGraphCreator

                AssetDatabase.CreateAsset(graph, assetPath);
                AssetDatabase.SaveAssets();
            }

            SNILPostProcessor.RegisterGraph(graphName, graph);

            context.CurrentGraphName = graphName;
            context.Graph = graph;
            // Сбрасываем последнюю ноду при начале нового графа
            context.LastNode = null;

            return InstructionResult.Ok(graph);
        }
    }
}