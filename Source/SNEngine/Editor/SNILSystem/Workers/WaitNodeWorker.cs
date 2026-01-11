using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Editor.SNILSystem.Workers
{
    public class WaitNodeWorker : SNILWorker
    {
        public override void ApplyParameters(BaseNode node, Dictionary<string, string> parameters)
        {
            // Check that this is actually a WaitNode or its subclass
            if (!(node is WaitNode waitNode))
            {
                return;
            }

            // Get all fields including from base classes
            var fields = GetAllFields(node.GetType());

            foreach (var kvp in parameters)
            {
                var field = fields.FirstOrDefault(f =>
                    f.Name.Equals(kvp.Key, System.StringComparison.OrdinalIgnoreCase) ||
                    f.Name.Equals("_" + kvp.Key, System.StringComparison.OrdinalIgnoreCase));

                if (field != null)
                {
                    object val = ConvertValue(kvp.Value, field.FieldType);
                    if (val != null || !field.FieldType.IsValueType)
                    {
                        field.SetValue(node, val);
                    }
                }
            }
        }

        private static FieldInfo[] GetAllFields(System.Type type)
        {
            var fields = new List<FieldInfo>();
            while (type != null && type != typeof(object))
            {
                fields.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                type = type.BaseType;
            }
            return fields.ToArray();
        }

        private static object ConvertValue(string value, System.Type targetType)
        {
            if (targetType == typeof(string)) return value;
            if (targetType == typeof(int)) return int.TryParse(value, out int i) ? i : 0;
            if (targetType == typeof(float)) return float.TryParse(value, out float f) ? f : 0f;
            if (targetType == typeof(bool)) return bool.TryParse(value, out bool b) ? b : false;
            if (targetType.IsEnum) return System.Enum.Parse(targetType, value, true);

            if (typeof(Object).IsAssignableFrom(targetType))
            {
                string filter = $"t:{targetType.Name} {value}";
                string[] guids = AssetDatabase.FindAssets(filter);
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    return AssetDatabase.LoadAssetAtPath(path, targetType);
                }
            }

            return null;
        }
    }
}