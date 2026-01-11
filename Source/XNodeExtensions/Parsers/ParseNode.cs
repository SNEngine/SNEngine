using System;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Parsers
{
    public abstract class ParseNode<TInput, TOutput> : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private TInput _input;

        [Output(ShowBackingValue.Never), SerializeField] private TOutput _output;

        protected object GetInputObject ()
        {
            Type type = GetInputPort(nameof(_input)).Connection.ValueType;

            return GetDataFromPort(nameof(_input), type);
        }
    }
}
