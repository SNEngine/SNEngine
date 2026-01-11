using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using SNEngine.Debugging;
using UnityEngine;
namespace SiphoinUnityHelpers.XNodeExtensions
{
    [NodeTint("#3b3b3b")]
    public class DebugNode : BaseNodeInteraction
    {
        [Space]

        [SerializeField, Input(ShowBackingValue.Never, ConnectionType.Override)] private Object _targetLog;

        [SerializeField, DebugNodeField, Input(connectionType = ConnectionType.Override)] private string _message;

        [Space]

        [SerializeField] private LogType _logType;

        public override void Execute()
        {
            object value = GetObjectForLog();

            switch (_logType)
            {
                case LogType.Message:
#if !SNEENGINE_SUPPORT
                    Debug.Log(value);

#else
                    NovelGameDebug.Log(value);
#endif
                    break;
                case LogType.Error:
#if !SNEENGINE_SUPPORT
                    Debug.LogError(value);

#else
                    NovelGameDebug.LogError(value);
#endif
                    break;
                case LogType.Warning:
#if !SNEENGINE_SUPPORT
                    Debug.LogWarning(value);

#else
                    NovelGameDebug.LogWarning(value);
#endif
                    break;
                default:
                    break;
            }
        }

        private object GetObjectForLog ()
        {
            string message = _message;

            object targetLog = _targetLog;

            var inputMessagePort = GetInputPort(nameof(_message));

            var objectTargetPort = GetInputPort(nameof(_targetLog));

            if (objectTargetPort.Connection != null)
            {
                targetLog = GetInputValue<object>(nameof(_targetLog));

                return targetLog;
            }

            if (inputMessagePort.Connection != null)
            {
                message = GetInputValue<string>(nameof(_message));

                return message;
            }
            if (!string.IsNullOrEmpty(message))
            {
                return message;
            }
            return null;


        }
    }
}
