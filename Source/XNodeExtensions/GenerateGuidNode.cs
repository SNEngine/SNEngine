using System;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    public class GenerateGuidNode : BaseNode
    {
        [SerializeField, Output(ShowBackingValue.Never)] private string _result;

        public override object GetValue(NodePort port)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
