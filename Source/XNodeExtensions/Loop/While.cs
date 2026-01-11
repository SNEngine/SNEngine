using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Loop
{
    [NodeTint("#593d6b")]
    public class WhileNode : AsyncNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private bool _condition;

        public override void Execute()
        {
            base.Execute();

            While().Forget();
        }

        private async UniTask While()
        {
            while (true)
            {
                bool condition = GetDataFromPort<bool>(nameof(_condition));

                if (condition)
                {
                    StopTask();

                    break;
                }

                await UniTask.Delay(1, cancellationToken: TokenSource.Token);
            }
        }
    }
}