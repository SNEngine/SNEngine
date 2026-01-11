using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem
{
    public class StringIntDictionaryVariableNode : DictionaryVariableNode<string, int>
    {
        [Output(ShowBackingValue.Always)] private StringStringDictionaryVariableNode _output;

        public override object GetValue(NodePort port) => this;
    }
}