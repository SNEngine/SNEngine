using SiphoinUnityHelpers.Exceptions;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    [NodeTint("#992f2f")]
    public class ErrorNode : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private NodeException _exception;

        [Input, SerializeField, ErrorNodeField] private string _message;

        

        public override void Execute()
        {
            var exception = GetDataFromPort<NodeException>(nameof(_exception));

            var message = _message;

            var messageInput = GetDataFromPort<string>(nameof(_message));

            if (messageInput != null)
            {
                message = messageInput;
            }

            if (exception != null)
            {

                LogError(exception.Message, exception.ThrowNode);

                return;
            }

            if (!string.IsNullOrEmpty(message))
            {
                LogError(message);
            }


        }

        private void LogError (string message, BaseNode node = null)
        {
            if (node != null)
            {
                Debug.LogError($"Node error from node {node.name} ({node.GUID} Error Message: {message})");
            }

            else
            {
                Debug.LogError($"Node error: {message}");
            }
            
        }
    }
}
