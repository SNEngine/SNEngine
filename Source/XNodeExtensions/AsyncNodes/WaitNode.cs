using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.AsyncNodes
{
    public class WaitNode : AsyncNodeWithSeconds
    {

        public override void Execute()
        {
            base.Execute();

            float seconds = GetSeconds();

            Wait(seconds).Forget();
        }

        private async UniTask Wait (float seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

            await UniTask.Delay(timeSpan);

            StopTask();


        }
    }
}
