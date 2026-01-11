using System;
using System.Reflection;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    [Serializable]
    public class NodePortEnumerable : NodePort
    {
        
        public NodePortEnumerable(FieldInfo fieldInfo) : base(fieldInfo)
        {

        }
    }
}
