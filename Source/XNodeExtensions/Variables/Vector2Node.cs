using Newtonsoft.Json.Linq;
using SNEngine.Debugging;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables
{
    [NodeTint("#494a52")]
    public class Vector2Node : VariableNode<Vector2>
    {
        public override void SetValue(object value)
        {
            if (value is null)
            {
                NovelGameDebug.LogError($"Vector2 Node error: value is null. Node: {GUID}");
                return;
            }

            if (value is Vector2 directValue)
            {
                base.SetValue(directValue);
                return;
            }

            if (value is JObject jObject)
            {
                try
                {
                    Vector2 result = jObject.ToObject<Vector2>();
                    base.SetValue(result);
                }
                catch
                {
                    NovelGameDebug.LogError($"Vector2 Node error: Failed to parse JObject. Node: {GUID}");
                }
                return;
            }

            NovelGameDebug.LogError($"Vector2 Node error: Unsupported type {value.GetType().Name}. Node: {GUID}");
        }
    }
}