using System;
using System.Reflection;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.AsyncNodes
{
    [Serializable]
    public class TimeOutPort : NodePort
    {
        public TimeOutPort(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
    }
}
