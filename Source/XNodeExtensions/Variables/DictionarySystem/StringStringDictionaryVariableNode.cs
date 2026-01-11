using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem
{
    public class StringStringDictionaryVariableNode : DictionaryVariableNode<string, string>
    {
        [Output(ShowBackingValue.Always)] private StringStringDictionaryVariableNode _output;

        public override object GetValue(NodePort port) => this;
    }
}