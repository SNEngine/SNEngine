using Cysharp.Threading.Tasks;
using SNEngine.AsyncNodes;
using System;
using System.Threading;

namespace SiphoinUnityHelpers.XNodeExtensions.Extensions
{
    public static class XNodeExtensionsUniTask
    {
        public static async UniTask WaitAsyncNode (IIncludeWaitingNode node, CancellationTokenSource cancellationTokenSource)
        {
            Func<bool> func = () => !node.IsWorking;

            if (cancellationTokenSource != null)
            {
                await UniTask.WaitUntil(func, cancellationToken: cancellationTokenSource.Token);
            }

            else
            {
                await UniTask.WaitUntil(func);
            }
        }
    }
}
