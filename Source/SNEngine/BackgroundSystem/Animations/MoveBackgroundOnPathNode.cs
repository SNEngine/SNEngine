using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine.BackgroundSystem.Animations
{
    public class MoveBackgroundOnPathNode : AsyncNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;
        [Input(ShowBackingValue.Unconnected), SerializeField] private PathType _pathType = PathType.CatmullRom;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Vector3[] _path;

        public override async void Execute()
        {
            base.Execute();
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);
            PathType inputPathType = GetInputValue(nameof(_pathType), _pathType);
            Vector3[] inputPath = GetInputValue(nameof(_path), _path);

            if (inputPath == null || inputPath.Length == 0)
            {
                Debug.LogError("Background path is empty.");
                StopTask();
                return;
            }

            var service = NovelGame.Instance.GetService<BackgroundService>();
            await service.MoveOnPath(inputPath, inputDuration, inputPathType, inputEase);
            StopTask();
        }
    }
}