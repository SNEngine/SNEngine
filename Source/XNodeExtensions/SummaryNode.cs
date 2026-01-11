using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    public class SummaryNode : DecoratorNode
    {
        [SerializeField, TextArea] private string _summary = "Write a summary...";

        public string Summary { get => _summary; set => _summary = value; }
    }
}
