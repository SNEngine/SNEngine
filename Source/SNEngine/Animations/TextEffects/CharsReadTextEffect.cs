using Cysharp.Threading.Tasks;
using SNEngine.Animations.TextEffects;
using TMPro;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CharsFadeIn : TextEffect
{
    public enum FadeStyle
    {
        Linear,
        Wave,
        Jump,
        Rainbow,
        JumpLinear
    }

    [SerializeField] private float _fadeDuration = 0.25f;
    [SerializeField] private float _delayBetweenChars = 0.05f;
    [SerializeField] private float _waveHeight = 10f;
    [SerializeField] private float _jumpHeight = 20f;
    [SerializeField] private FadeStyle _fadeStyle;
    [SerializeField] private float _animationDelay = 0f;

    private NativeArray<Color32> _colorsFlat;
    private NativeArray<int> _meshStartIndices;
    private NativeArray<int> _meshVertexCounts;
    private NativeArray<float3> _verticesFlat;
    private NativeArray<float3> _baseVerticesFlat;

    private bool _loopRunning;
    private bool _forceCompleted;

    private struct CharInfoUnmanaged
    {
        public int isVisible;
        public int materialReferenceIndex;
        public int vertexIndex;
    }

    [BurstCompile]
    private struct FadeJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<CharInfoUnmanaged> charInfos;
        [ReadOnly] public float currentTime;
        [ReadOnly] public float startTime;
        [ReadOnly] public float duration;
        [ReadOnly] public float delayBetween;
        [ReadOnly] public int style;
        [ReadOnly] public float waveHeight;
        [ReadOnly] public float jumpHeight;

        [NativeDisableParallelForRestriction] public NativeArray<Color32> colorsFlat;
        [NativeDisableParallelForRestriction] public NativeArray<float3> verticesFlat;
        [ReadOnly] public NativeArray<float3> baseVerticesFlat;

        [ReadOnly] public NativeArray<int> meshStartIndices;
        [ReadOnly] public NativeArray<int> meshVertexCounts;

        public void Execute(int index)
        {
            if (index >= charInfos.Length) return;
            var ci = charInfos[index];
            if (ci.isVisible == 0) return;

            float charStart = startTime + index * delayBetween;
            float t = math.clamp((currentTime - charStart) / duration, 0f, 1f);

            byte alpha = 0;
            float offsetY = 0f;

            if (style == 0)
            {
                alpha = (byte)(t * 255f);
            }
            else if (style == 1)
            {
                alpha = (byte)(t * 255f);
                offsetY = math.sin(t * math.PI) * waveHeight;
            }
            else if (style == 2)
            {
                alpha = (byte)(t * 255f);
                offsetY = math.sin(t * math.PI * 2f) * jumpHeight;
            }
            else if (style == 3)
            {
                float hue = math.frac((index * 0.1f) + currentTime * 0.6f);
                float3 rgb = HSVToRGB(hue, 1f, 1f);
                int meshIndex = ci.materialReferenceIndex;
                if (meshIndex < 0 || meshIndex >= meshStartIndices.Length) return;
                int baseIdx = meshStartIndices[meshIndex];
                int dst = baseIdx + ci.vertexIndex;
                if (dst < 0 || (dst + 3) >= verticesFlat.Length) return;

                byte a = (byte)(t * 255f);
                for (int j = 0; j < 4; j++)
                {
                    var bv = baseVerticesFlat[dst + j];
                    verticesFlat[dst + j] = bv;
                    colorsFlat[dst + j] = new Color32((byte)(rgb.x * 255f), (byte)(rgb.y * 255f), (byte)(rgb.z * 255f), a);
                }
                return;
            }
            else if (style == 4)
            {
                float jumpT = math.clamp((currentTime - charStart) / duration, 0f, 1f);
                alpha = (byte)(jumpT * 255f);
                offsetY = math.sin(jumpT * math.PI) * jumpHeight;
            }

            int meshIdx = ci.materialReferenceIndex;
            int vIndex = ci.vertexIndex;
            if (meshIdx < 0 || meshIdx >= meshStartIndices.Length) return;

            int baseIndex = meshStartIndices[meshIdx];
            int meshLen = meshVertexCounts[meshIdx];
            if (vIndex < 0 || (vIndex + 3) >= meshLen) return;

            int dstIdx = baseIndex + vIndex;
            if (dstIdx < 0 || (dstIdx + 3) >= verticesFlat.Length) return;

            for (int j = 0; j < 4; j++)
            {
                var c = colorsFlat[dstIdx + j];
                colorsFlat[dstIdx + j] = new Color32(c.r, c.g, c.b, alpha);
                var bv = baseVerticesFlat[dstIdx + j];
                verticesFlat[dstIdx + j] = new float3(bv.x, bv.y + offsetY, bv.z);
            }
        }

        private static float3 HSVToRGB(float h, float s, float v)
        {
            int i = (int)(h * 6f);
            float f = h * 6f - i;
            float p = v * (1f - s);
            float q = v * (1f - f * s);
            float t = v * (1f - (1f - f) * s);
            switch (i % 6)
            {
                case 0: return new float3(v, t, p);
                case 1: return new float3(q, v, p);
                case 2: return new float3(p, v, t);
                case 3: return new float3(p, q, v);
                case 4: return new float3(t, p, v);
                default: return new float3(v, p, q);
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartAsync().Forget();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        DisposeFlatBuffersIfCreated();
    }

    private void OnDestroy()
    {
        DisposeFlatBuffersIfCreated();
    }

    protected override void TextForceCompleted(TextMeshProUGUI textMesh)
    {
        _forceCompleted = true;
        if (_loopRunning)
        {
            if (_verticesFlat.IsCreated && _baseVerticesFlat.IsCreated)
            {
                for (int i = 0; i < _verticesFlat.Length; i++)
                {
                    _verticesFlat[i] = _baseVerticesFlat[i];
                }
            }
            SetAllAlphaImmediate(255);
            ApplyToMesh();
        }
    }

    private async UniTaskVoid StartAsync()
    {
        Component.color = Color.clear;
        await UniTask.Yield();
        Component.ForceMeshUpdate(true, true);
        Component.color = Color.white;
        StartFadeIn();
    }

    private void StartFadeIn()
    {
        _forceCompleted = false;
        Component.ForceMeshUpdate(true, true);
        BuildFlatBuffers();
        SetAllAlphaImmediate(0);
        ApplyToMesh();
        if (!_loopRunning)
        {
            _loopRunning = true;
            FadeInText().Forget();
        }
    }

    private void BuildFlatBuffers()
    {
        DisposeFlatBuffersIfCreated();
        var meshInfo = Component.textInfo.meshInfo;
        int chunks = meshInfo.Length;
        int totalVertices = 0;
        int[] vertexCounts = new int[chunks];
        for (int i = 0; i < chunks; i++)
        {
            int vc = meshInfo[i].vertices != null ? meshInfo[i].vertices.Length : 0;
            vertexCounts[i] = vc;
            totalVertices += vc;
        }

        _colorsFlat = new NativeArray<Color32>(totalVertices, Allocator.Persistent);
        _verticesFlat = new NativeArray<float3>(totalVertices, Allocator.Persistent);
        _baseVerticesFlat = new NativeArray<float3>(totalVertices, Allocator.Persistent);
        _meshStartIndices = new NativeArray<int>(chunks, Allocator.Persistent);
        _meshVertexCounts = new NativeArray<int>(chunks, Allocator.Persistent);

        int offset = 0;
        for (int i = 0; i < chunks; i++)
        {
            _meshStartIndices[i] = offset;
            _meshVertexCounts[i] = vertexCounts[i];

            var srcColors = meshInfo[i].colors32;
            var verts = meshInfo[i].vertices;
            int len = vertexCounts[i];

            for (int v = 0; v < len; v++)
            {
                _colorsFlat[offset + v] = srcColors != null && srcColors.Length == len ? srcColors[v] : new Color32(255, 255, 255, 255);
                float3 baseV = verts != null && verts.Length == len ? verts[v] : float3.zero;
                _baseVerticesFlat[offset + v] = baseV;
                _verticesFlat[offset + v] = baseV;
            }
            offset += len;
        }
    }

    private void DisposeFlatBuffersIfCreated()
    {
        if (_colorsFlat.IsCreated) _colorsFlat.Dispose();
        if (_verticesFlat.IsCreated) _verticesFlat.Dispose();
        if (_baseVerticesFlat.IsCreated) _baseVerticesFlat.Dispose();
        if (_meshStartIndices.IsCreated) _meshStartIndices.Dispose();
        if (_meshVertexCounts.IsCreated) _meshVertexCounts.Dispose();
    }

    private void ApplyToMesh()
    {
        if (Component == null || !_verticesFlat.IsCreated || !_colorsFlat.IsCreated) return;
        var meshInfo = Component.textInfo.meshInfo;
        int chunks = meshInfo.Length;
        for (int i = 0; i < chunks; i++)
        {
            int start = _meshStartIndices[i];
            int len = _meshVertexCounts[i];
            if (len == 0)
            {
                meshInfo[i].colors32 = null;
                continue;
            }
            Color32[] arr = new Color32[len];
            Vector3[] verts = new Vector3[len];
            for (int v = 0; v < len; v++)
            {
                arr[v] = _colorsFlat[start + v];
                verts[v] = _verticesFlat[start + v];
            }
            meshInfo[i].colors32 = arr;
            meshInfo[i].vertices = verts;
        }
        Component.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32 | TMP_VertexDataUpdateFlags.Vertices);
    }

    private void SetAllAlphaImmediate(byte alpha)
    {
        if (!_colorsFlat.IsCreated) return;
        for (int i = 0; i < _colorsFlat.Length; i++)
        {
            var c = _colorsFlat[i];
            _colorsFlat[i] = new Color32(c.r, c.g, c.b, alpha);
        }
    }

    private async UniTask FadeInText()
    {
        if (_animationDelay > 0f)
            await UniTask.Delay(System.TimeSpan.FromSeconds(_animationDelay));

        var info = Component.textInfo;
        int totalChars = info.characterCount;
        if (totalChars == 0)
        {
            _loopRunning = false;
            ResetFlagAnimation();
            return;
        }

        NativeArray<CharInfoUnmanaged> charArr = new NativeArray<CharInfoUnmanaged>(totalChars, Allocator.TempJob);
        for (int i = 0; i < totalChars; i++)
        {
            var ci = info.characterInfo[i];
            charArr[i] = new CharInfoUnmanaged
            {
                isVisible = ci.isVisible ? 1 : 0,
                materialReferenceIndex = ci.materialReferenceIndex,
                vertexIndex = ci.vertexIndex
            };
        }

        float startTime = Time.time;

        while (!_forceCompleted)
        {
            if (Component == null)
                break;
            if (!VerifyBuffersMatchMesh())
            {
                BuildFlatBuffers();
            }

            float currentTime = Time.time;

            var job = new FadeJob
            {
                charInfos = charArr,
                currentTime = currentTime,
                startTime = startTime,
                duration = _fadeDuration,
                delayBetween = _delayBetweenChars,
                colorsFlat = _colorsFlat,
                verticesFlat = _verticesFlat,
                baseVerticesFlat = _baseVerticesFlat,
                meshStartIndices = _meshStartIndices,
                meshVertexCounts = _meshVertexCounts,
                style = (int)_fadeStyle,
                waveHeight = _waveHeight,
                jumpHeight = _jumpHeight
            };

            JobHandle handle = job.Schedule(totalChars, 64);
            handle.Complete();

            ApplyToMesh();

            float elapsed = currentTime - startTime;
            if (elapsed > _fadeDuration + totalChars * _delayBetweenChars)
                break;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        if (!_forceCompleted)
        {
            SetAllAlphaImmediate(255);
            ApplyToMesh();
        }

        charArr.Dispose();
        _loopRunning = false;
        ResetFlagAnimation();
    }

    private bool VerifyBuffersMatchMesh()
    {
        var meshInfo = Component.textInfo.meshInfo;
        if (!_meshVertexCounts.IsCreated) return false;
        if (_meshVertexCounts.Length != meshInfo.Length) return false;

        int expectedTotal = 0;
        for (int i = 0; i < meshInfo.Length; i++)
        {
            int vc = meshInfo[i].vertices != null ? meshInfo[i].vertices.Length : 0;
            if (vc != _meshVertexCounts[i]) return false;
            expectedTotal += vc;
        }
        return expectedTotal == _colorsFlat.Length;
    }
}
