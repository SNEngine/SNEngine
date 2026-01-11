using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.BackgroundSystem;
using SNEngine.Debugging;
using UnityEngine;
using Object = UnityEngine.Object;
using SNEngine.Animations;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Bavkground Service")]
    public class BackgroundService : ServiceBase
    {
        private IBackgroundRenderer background;

        public override void Initialize()
        {
            var backgroundAsset = Resources.Load<BackgroundRenderer>("Render/Background");

            var screenBackground = Resources.Load<ScreenBackgroundRender>("Render/ScreenBackground");

            var screenBackgroundPrefab = Instantiate(screenBackground);

            screenBackgroundPrefab.name = screenBackground.name;

            Object.DontDestroyOnLoad(screenBackgroundPrefab);

            var backgroundPrefab = Instantiate(backgroundAsset);

            backgroundPrefab.name = backgroundAsset.name;

            Object.DontDestroyOnLoad(backgroundPrefab);

            background = backgroundPrefab;
        }

        public override void ResetState()
        {
            background.ResetState();
        }

        public void Set(Sprite sprite)
        {
            if (sprite is null)
            {
                NovelGameDebug.LogError($"Sprite for set background not seted. Check your graph");
            }

            background.SetData(sprite);
        }

        public void Clear()
        {
            background.Clear();
        }

        #region Animations

        public async UniTask SetTransperent(float fadeValue, float duration, Ease ease)
        {
            await background.SetTransperent(fadeValue, duration, ease);
        }

        public async UniTask SetColor(Color color, float duration, Ease ease)
        {
            await background.SetColor(color, duration, ease);
        }

        public async UniTask SetBrightness(float brightnessValue, float duration, Ease ease)
        {
            await background.SetBrightness(brightnessValue, duration, ease);
        }

        public async UniTask MoveTo(Vector3 position, float duration, Ease ease)
        {
            await background.MoveTo(position, duration, ease);
        }

        public async UniTask LocalMoveTo(Vector3 localPosition, float duration, Ease ease)
        {
            await background.LocalMoveTo(localPosition, duration, ease);
        }

        public async UniTask RotateTo(Vector3 rotation, float duration, Ease ease)
        {
            await background.RotateTo(rotation, duration, ease);
        }

        public async UniTask LocalRotateTo(Vector3 localRotation, float duration, Ease ease)
        {
            await background.LocalRotateTo(localRotation, duration, ease);
        }

        public async UniTask ScaleTo(Vector3 scale, float duration, Ease ease)
        {
            await background.ScaleTo(scale, duration, ease);
        }

        public async UniTask PunchPosition(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            await background.PunchPosition(punch, duration, vibrato, elasticity);
        }

        public async UniTask PunchRotation(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            await background.PunchRotation(punch, duration, vibrato, elasticity);
        }

        public async UniTask PunchScale(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            await background.PunchScale(punch, duration, vibrato, elasticity);
        }

        public async UniTask ShakePosition(float duration, float strength = 90, int vibrato = 10, bool fadeOut = true)
        {
            await background.ShakePosition(duration, strength, vibrato, fadeOut);
        }

        public async UniTask ShakeRotation(float duration, float strength = 90, int vibrato = 10, bool fadeOut = true)
        {
            await background.ShakeRotation(duration, strength, vibrato, fadeOut);
        }

        public async UniTask ShakeScale(float duration, float strength = 1, int vibrato = 10, float fadeOut = 0)
        {
            await background.ShakeScale(duration, strength, vibrato, fadeOut);
        }

        public async UniTask MoveOnPath(Vector3[] path, float duration, PathType pathType = PathType.CatmullRom, Ease ease = Ease.Linear)
        {
            await background.MoveOnPath(path, duration, pathType, ease);
        }

        public async UniTask LookAtTarget(Vector3 worldPosition, float duration, Ease ease)
        {
            await background.LookAtTarget(worldPosition, duration, ease);
        }

        public void SetLoopingMove(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear)
        {
            background.SetLoopingMove(endValue, duration, loopType, ease);
        }

        public void SetLoopingRotate(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear)
        {
            background.SetLoopingRotate(endValue, duration, loopType, ease);
        }

        public void SetLoopingScale(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear)
        {
            background.SetLoopingScale(endValue, duration, loopType, ease);
        }

        public async UniTask Vignette(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            await background.Vignette(time, animationBehaviour, ease);
        }

        public async UniTask Vignette(float time, float value, Ease ease)
        {
            await background.Vignette(time, value, ease);
        }

        public async UniTask StarWipe(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            await background.StarWipe(time, animationBehaviour, ease);
        }

        public async UniTask StarWipe(float time, float value, Ease ease)
        {
            await background.StarWipe(time, value, ease);
        }


        public async UniTask Dissolve(float time, AnimationBehaviourType animationBehaviour, Ease ease, Texture2D texture = null)
        {
            await background.Dissolve(time, animationBehaviour, ease, texture);
        }

        public async UniTask ToBlackAndWhite(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            await background.ToBlackAndWhite(time, animationBehaviour, ease);
        }

        public async UniTask ToBlackAndWhite(float time, float value, Ease ease)
        {
            await background.ToBlackAndWhite(time, value, ease);
        }

        public async UniTask Celia(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            await background.Celia(time, animationBehaviour, ease);
        }

        public async UniTask Celia(float time, float value, Ease ease)
        {
            await background.Celia(time, value, ease);
        }

        public async UniTask Solid(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            await background.Solid(time, animationBehaviour, ease);
        }

        public async UniTask Solid(float time, float value, Ease ease)
        {
            await background.Solid(time, value, ease);
        }

        public async UniTask Illuminate(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            await background.Illuminate(time, animationBehaviour, ease);
        }

        public async UniTask Illuminate(float time, float value, Ease ease)
        {
            await background.Illuminate(time, value, ease);
        }

        #endregion
    }
}