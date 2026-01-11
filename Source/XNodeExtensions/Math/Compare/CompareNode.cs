using System;
using System.Data;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Math.Compare
{
    [NodeTint("#3d6b6b")]
    public abstract class CompareNode : BaseNode
    {

        [SerializeField, Output(ShowBackingValue.Never)] private bool _result;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return null;
            }
            return Compute();

        }

        protected abstract bool Compute();

        
    }


}
