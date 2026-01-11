using SNEngine.Debugging;
using SNEngine.Serialisation;
using SNEngine.Serialization;
using Newtonsoft.Json.Linq;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables
{
    public abstract class UnityVariableNode<T, TLibrary> : VariableNode<T> where T : UnityEngine.Object where TLibrary : BaseAssetLibrary<T>
    {
        public override void SetValue(object value)
        {
            if (value is null)
            {
                NovelGameDebug.LogError($"Unity Variable Node error: value is null. Error from node {GUID}");
                return;
            }

            if (value is T original)
            {
                base.SetValue(original);
                return;
            }

            if (value is string guid)
            {
                var result = SNEngineSerialization.GetFromLibrary<TLibrary, T>(guid);
                base.SetValue(result);
                return;
            }

            if (value is JObject jObject)
            {
                string extractedGuid = jObject["GUID"]?.ToString() ?? jObject["guid"]?.ToString();

                if (!string.IsNullOrEmpty(extractedGuid))
                {
                    var result = SNEngineSerialization.GetFromLibrary<TLibrary, T>(extractedGuid);
                    base.SetValue(result);
                }
                else
                {
                    NovelGameDebug.LogError($"Unity Variable Node error: JObject does not contain a valid GUID. Error from node {GUID}");
                }
                return;
            }

            NovelGameDebug.LogError($"Unity Variable Node error: value invalid. Type: {value.GetType().Name} Error from node {GUID}");
        }
    }
}