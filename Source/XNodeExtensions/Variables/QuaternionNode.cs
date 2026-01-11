using Newtonsoft.Json.Linq;
using SNEngine.Debugging;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables
{
    [NodeTint("#524949")]
    public class QuaternionNode : VariableNode<Quaternion>
    {
        public override void SetValue(object value)
        {
            if (value is null)
            {
                NovelGameDebug.LogError($"Quaternion Node error: value is null. Node: {GUID}");
                return;
            }

            if (value is Quaternion directValue)
            {
                base.SetValue(directValue);
                return;
            }

            if (value is JObject jObject)
            {
                try
                {
                    Quaternion result = jObject.ToObject<Quaternion>();
                    base.SetValue(result);
                }
                catch
                {
                    NovelGameDebug.LogError($"Quaternion Node error: Failed to parse JObject. Node: {GUID}");
                }
                return;
            }

            NovelGameDebug.LogError($"Quaternion Node error: Unsupported type {value.GetType().Name}. Node: {GUID}");
        }
    }
}