using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
using Cysharp.Threading.Tasks;
using System;
using SNEngine.Audio;
using TMPro;
using DG.Tweening;

#if UNITY_WEBGL
using SNEngine.WebGL;
#endif

namespace CoreGame
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(VideoPlayer))]
    [RequireComponent(typeof(AspectRatioFitter))]
    [RequireComponent(typeof(AudioSourceController))]
#if UNITY_WEBGL
    [RequireComponent(typeof(WebGLVideoPlayerAudioSourceController))]
#endif
    public class MenuIntroPlayer : MonoBehaviour
    {
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AspectRatioFitter _aspectRatioFitter;
        [SerializeField] private TextMeshProUGUI _loadingText;

        private RenderTexture _renderTexture;
        private bool _isUserInteracted = false;

        [Header("Video Settings")]
        [SerializeField] private string _videoFilePath;
        [SerializeField] private bool _playOnAwake = true;
        [SerializeField] private bool _isLooping = true;

        private void OnValidate()
        {
            if (!_videoPlayer) _videoPlayer = GetComponent<VideoPlayer>();
            if (!_audioSource) _audioSource = GetComponent<AudioSource>();
            if (!_rawImage) _rawImage = GetComponent<RawImage>();
            if (!_aspectRatioFitter) _aspectRatioFitter = GetComponent<AspectRatioFitter>();
        }

        private void Start()
        {
            SetupVideoPlayer();

            if (!string.IsNullOrEmpty(_videoFilePath))
            {
                SetVideoUrl(_videoFilePath);
            }

            _videoPlayer.isLooping = _isLooping;

            if (_playOnAwake && !string.IsNullOrEmpty(_videoPlayer.url))
            {
                _videoPlayer.Prepare();
            }
        }

        private void Update()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer && !_isUserInteracted)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    _isUserInteracted = true;
                }
            }
        }

        private void OnVideoPrepared(VideoPlayer source)
        {
            if (_aspectRatioFitter)
            {
                _aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;

                float aspectRatio = (float)source.width / source.height;
                _aspectRatioFitter.aspectRatio = aspectRatio;

                if (source.clip == null && !string.IsNullOrEmpty(source.url))
                {
                    UpdateRenderTexture((int)source.width, (int)source.height);
                }
            }

            if (Application.platform != RuntimePlatform.WebGLPlayer && _playOnAwake)
            {
                source.Play();
            }
        }

        private void OnVideoStarted(VideoPlayer source)
        {
            if (_loadingText != null)
            {
                _loadingText.DOFade(0, 0.5f).OnComplete(() => _loadingText.gameObject.SetActive(false));
            }
        }

        private void SetupVideoPlayer()
        {
            _renderTexture = new RenderTexture(1280, 720, 24);
            _renderTexture.Create();

            _rawImage.texture = _renderTexture;

            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.targetTexture = _renderTexture;
            _videoPlayer.waitForFirstFrame = true;

            _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            _videoPlayer.SetTargetAudioSource(0, _audioSource);
            _audioSource.playOnAwake = false;

            _videoPlayer.prepareCompleted += OnVideoPrepared;
            _videoPlayer.started += OnVideoStarted;

            if (_loadingText != null)
            {
                _loadingText.gameObject.SetActive(true);
                _loadingText.DOFade(1, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }
        }

        private void UpdateRenderTexture(int width, int height)
        {
            if (_renderTexture == null || _renderTexture.width != width || _renderTexture.height != height)
            {
                if (_renderTexture != null)
                {
                    _renderTexture.Release();
                    Destroy(_renderTexture);
                }

                _renderTexture = new RenderTexture(width, height, 24);
                _renderTexture.Create();
                _rawImage.texture = _renderTexture;
                _videoPlayer.targetTexture = _renderTexture;
            }
        }

        private void SetVideoUrl(string relativePath)
        {
            string path = Path.Combine(Application.streamingAssetsPath, relativePath);

            _videoPlayer.source = VideoSource.Url;
            _videoPlayer.url = path;
        }

        private void PrepareAndPlay()
        {
            if (_videoPlayer.isPrepared)
            {
                _videoPlayer.Play();
            }
            else
            {
                _videoPlayer.Prepare();
            }
        }

        public async UniTask StartVideoAfterDelay(TimeSpan delay)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                await UniTask.WaitUntil(() => _isUserInteracted);
            }

            if (!_videoPlayer.isPrepared)
            {
                _videoPlayer.Prepare();
                await UniTask.WaitUntil(() => _videoPlayer.isPrepared);
            }

            await UniTask.Delay(delay);

            _videoPlayer.Play();
        }

        private void OnDestroy()
        {
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