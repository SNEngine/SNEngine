using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Debugging;
using SNEngine.Graphs;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Variable Container Service")]
    public class VariablesContainerService : ServiceBase, IContainerVariables
    {
        private IContainerVariables _containerVariables;
        public IDictionary<string, VariableNode> GlobalVariables => _containerVariables.GlobalVariables;

        private const string PATH = "VariableContainerGraph";

        public override void Initialize()
        {
            var graph = Resources.Load<VariableContainerGraph>(PATH);

            if (graph != null)
            {
                NovelGameDebug.Log($"{nameof(GlobalVariables)} Container loaded. Build Variables...");

                graph.Execute();

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"{nameof(GlobalVariables)} Container finished Build.");

                foreach (var item in graph.GlobalVariables)
                {
                    stringBuilder.AppendLine($"Key: {item.Key} Value: {item.Value.GetCurrentValue()} Type Node: {item.Value.GetType().Name}");
                }


                NovelGameDebug.Log(stringBuilder.ToString());

                _containerVariables = graph;
            }

            else
            {
                NovelGameDebug.LogError($"{nameof(GlobalVariables)} Container not found on path {PATH}");
            }
        }
    }
}
