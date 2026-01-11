using System;
using System.Reflection;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Loop
{
    [Serializable]
    public class LoopPort : NodePort
    {
        public LoopPort(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
    }
}
