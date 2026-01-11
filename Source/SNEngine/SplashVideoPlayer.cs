using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;

namespace SNEngine
{
    [RequireComponent(typeof(VideoPlayer))]
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(AspectRatioFitter))]
    public class SplashVideoPlayer : MonoBehaviour
    {
        private const string VIDEO_PATH = "SNEngine_Splash_Screen.mp4";
        private const string MAIN_SCENE_NAME = "Main";

        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private AspectRatioFitter _aspectRatioFitter;

        private RenderTexture _renderTexture;

        private void OnValidate()
        {
            if (!_videoPlayer) _videoPlayer = GetComponent<VideoPlayer>();
            if (!_rawImage) _rawImage = GetComponent<RawImage>();
            if (!_aspectRatioFitter) _aspectRatioFitter = GetComponent<AspectRatioFitter>();
        }

        private void Start()
        {
            if (!SNEngineRuntimeSettings.Instance.ShowVideoSplash)
            {
                _rawImage.enabled = false;
                LoadMainScene();
                return;
            }

            Initialize();
        }

        private void Initialize()
        {
            _videoPlayer.source = VideoSource.Url;
            _videoPlayer.url = Path.Combine(Application.streamingAssetsPath, "Splash", VIDEO_PATH);
            _videoPlayer.isLooping = false;
            _videoPlayer.playOnAwake = false;
            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

            _videoPlayer.prepareCompleted += OnPrepared;
            _videoPlayer.loopPointReached += OnFinished;

            _videoPlayer.Prepare();
        }

        private void OnPrepared(VideoPlayer source)
        {
            _renderTexture = new RenderTexture((int)source.width, (int)source.height, 24);
            _renderTexture.Create();

            _rawImage.texture = _renderTexture;
            _videoPlayer.targetTexture = _renderTexture;

            if (_aspectRatioFitter != null)
            {
                _aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                _aspectRatioFitter.aspectRatio = (float)source.width / source.height;
            }

            source.Play();
        }

        private void OnFinished(VideoPlayer source) => LoadMainScene();

        private void LoadMainScene()
        {
            SceneManager.LoadScene(MAIN_SCENE_NAME);
        }

        private void OnDestroy()
        {
            _videoPlayer.prepareCompleted -= OnPrepared;
            _videoPlayer.loopPointReached -= OnFinished;

            if (_renderTexture != null)
            {
                if (_renderTexture.IsCreated())
                {
                    _renderTexture.Release();
                }
                Destroy(_renderTexture);
                _renderTexture = null;
            }
        }
    }
}