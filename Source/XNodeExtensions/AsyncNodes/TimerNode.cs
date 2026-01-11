using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers.XNodeExtensions.Interfaces;
using System;
using System.Threading;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.AsyncNodes
{
    [NodeTint("#5c2b3f")]
    public class TimerNode : AsyncNodeWithSeconds, ILoopNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private bool _tickOnStart = true;

        [SerializeField] private TimerType _type;

        [Output, SerializeField] private TimeOutPort _timeOut;

        [Output, SerializeField] private TimerNode _someTimer;

        private CancellationTokenSource _cancellationTokenSource;


        public override object GetValue(NodePort port)
        {
            return this;
        }

        public override void Execute()
        {
            bool tickOnStart = _tickOnStart;

            var inputTickOnStart = GetInputPort(nameof(_tickOnStart));

            if (inputTickOnStart.Connection != null)
            {
                tickOnStart = GetDataFromPort<bool>(nameof(_tickOnStart));
            }

            if (tickOnStart)
            {
                Start();
            }
        }

        private async UniTask TimeOut (float seconds)
        {
            if (_type == TimerType.One)
            {
                await Delay(seconds);

                ExecuteInTimer();

                Stop();
            }

            else if (_type == TimerType.Repeat)
            {
                while (true)
                {
                    await Delay(seconds);

                    ExecuteInTimer();
                }
            }
        }

        private void ExecuteInTimer()
        {
            var timeOutPort = GetOutputPort(nameof(_timeOut));

            var connections = timeOutPort.GetConnections();

            if (connections != null)
            {
                foreach (var connection in connections)
                {
                    var node = connection.node as BaseNodeInteraction;

                    node.Execute();
                }
            }
        }

        public void Stop ()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void Start ()
        {
            Stop();

            _cancellationTokenSource = new CancellationTokenSource();

            float seconds = GetSeconds();

            TimeOut(seconds).Forget();
        }

        private async UniTask Delay (float seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

            await UniTask.Delay(timeSpan, cancellationToken: _cancellationTokenSource.Token);
        }

        public bool NodeContainsOnLoop(BaseNodeInteraction node)
        {
            var port = GetOutputPort(nameof(_timeOut));

            if (port.ConnectionCount > 0)
            {
                var connections = port.GetConnections();

                return connections.Contains(node.GetEnterPort());
            }

            return false;
        }
    }
}
