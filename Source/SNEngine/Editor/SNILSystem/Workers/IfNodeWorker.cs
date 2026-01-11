using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Editor.SNILSystem.Workers
{
    public class IfNodeWorker : SNILWorker
    {
        public override void ApplyParameters(BaseNode node, Dictionary<string, string> parameters)
        {
            // Проверяем, что это действительно IfNode или его наследник
            if (!(node is IfNode ifNode))
            {
                SNILDebug.LogError($"Node {node.GetType().Name} is not an IfNode");
                return;
            }

            // Устанавливаем параметры для IfNode
            foreach (var param in parameters)
            {
                // IfNode обычно получает условие через входной порт, а не через сериализованные поля
                // Вместо этого, условие будет подключено из CompareIntegersNode
                SNILDebug.Log($"IfNode parameter: {param.Key} = {param.Value}");
            }
        }
    }
}