using UnityEngine;
using System;

namespace SNEngine.DialogSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class OldRenderDialogue : MonoBehaviour, IOldRenderDialogue
    {
        private SpriteRenderer _spriteRenderer;

        private Camera _camera;

        private void Awake()
        {
            if (!TryGetComponent(out _spriteRenderer))
            {
                throw new NullReferenceException("sprite renderer null");
            }

            _camera = Camera.main;
        }

        public Texture2D UpdateRender()
        {
            var mCamera = _camera;

            Rect rect = new Rect(0, 0, mCamera.pixelWidth, mCamera.pixelHeight);
            RenderTexture renderTexture = new RenderTexture(mCamera.pixelWidth, mCamera.pixelHeight, 24);
            Texture2D screenShot = new Texture2D(mCamera.pixelWidth, mCamera.pixelHeight, TextureFormat.RGBA32, false);

            mCamera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            mCamera.Render();

            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            mCamera.targetTexture = null;
            RenderTexture.active = null;

            Destroy(renderTexture);

            return screenShot;
        }

        public void DisplayFrame(Texture2D frameTexture)
        {
            gameObject.SetActive(true);
            Sprite newSprite = Sprite.Create(
                frameTexture,
                new Rect(0, 0, frameTexture.width, frameTexture.height),
                Vector2.one * 0.5f,
                100f,
                0,
                SpriteMeshType.FullRect
            );

            _spriteRenderer.sprite = newSprite;

            if (_camera.orthographic)
            {
                float cameraHeight = _camera.orthographicSize * 2f;
                float cameraWidth = cameraHeight * _camera.aspect;

                float spriteWorldWidth = frameTexture.width / 100f;
                float spriteWorldHeight = frameTexture.height / 100f;

                float scaleX = cameraWidth / spriteWorldWidth;
                float scaleY = cameraHeight / spriteWorldHeight;

                transform.position = _camera.transform.position + _camera.transform.forward * 1f;
                transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }

            _spriteRenderer.enabled = true;

            if (_spriteRenderer.sprite != null && _spriteRenderer.sprite != newSprite)
            {
                Destroy(_spriteRenderer.sprite);
            }
        }

        public void HideFrame()
        {
            _spriteRenderer.enabled = false;

            if (_spriteRenderer.sprite != null)
            {
                Destroy(_spriteRenderer.sprite);
                _spriteRenderer.sprite = null;
            }
        }

        public void ResetState()
        {
            _spriteRenderer.enabled = false;

            if (_spriteRenderer.sprite != null)
            {
                Destroy(_spriteRenderer.sprite);
                _spriteRenderer.sprite = null;
            }
        }
    }
    }