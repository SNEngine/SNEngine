using System.Text;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Build String")]
    public class BuildStringNode : BaseNode
    {
        [Input(ShowBackingValue.Never), SerializeField] private string _strings;

        [Output(ShowBackingValue.Never), SerializeField] private string _result;

        [SerializeField] private BuildStringType _type;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return null;
            }

            var connections = GetInputPort(nameof(_strings)).GetConnections();

            string[] strings = new string[connections.Count];

            for ( int i = 0; i < connections.Count; i++ )
            {
                strings[i] = connections[i].GetOutputValue() as string;
            }

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in strings)
            {
                switch (_type)
                {
                    case BuildStringType.Append:
                        stringBuilder.Append(item);
                        break;
                    case BuildStringType.AppendLine:
                        stringBuilder.AppendLine(item);
                        break;
                }
            }

            return stringBuilder.ToString();
        }
    }
}
