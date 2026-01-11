using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Finding.FindByType
{
    public abstract class FindWithTypeNode<TResult> : BaseNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _typeName;

        [Output(ShowBackingValue.Never), SerializeField] private TResult _result;

        public override object GetValue(NodePort port)
        {
            string typeName = _typeName;

            var input = GetInputPort(nameof(_typeName));

            if (input.Connection != null)
            {
                typeName = GetDataFromPort<string>(nameof(_typeName));
            }

            Type type = FinderType.Find(typeName);

            return ReturnResult(type);

        }

        protected abstract TResult ReturnResult(Type type);
    }
}
