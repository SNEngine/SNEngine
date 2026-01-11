Shader "URP/SpriteDifferenceBlend"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _BlendTex ("Blend Texture", 2D) = "white" {}

        _Color ("Tint", Color) = (1,1,1,1)
        _Blend ("Blend", Range(0,1)) = 0

        _DiffThreshold ("Diff Threshold", Range(0,0.5)) = 0.08
        _DiffPower ("Diff Power", Range(1,8)) = 3
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "RenderPipeline"="UniversalPipeline"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "SpriteDifferenceBlend"
            Tags
            {
                // ¬ј∆Ќќ: этот LightMode работает и в 2D Renderer, и в Forward
                "LightMode"="SRPDefaultUnlit"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_BlendTex);
            SAMPLER(sampler_BlendTex);

            float4 _Color;
            float _Blend;
            float _DiffThreshold;
            float _DiffPower;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 baseCol  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 blendCol = SAMPLE_TEXTURE2D(_BlendTex, sampler_BlendTex, i.uv);

                // difference-based pseudo mask
                half diff =
                    abs(baseCol.r - blendCol.r) +
                    abs(baseCol.g - blendCol.g) +
                    abs(baseCol.b - blendCol.b);

                diff = saturate((diff - _DiffThreshold) * 4.0);
                diff = pow(diff, _DiffPower);

                half t = saturate(diff * _Blend);

                half4 col = lerp(baseCol, blendCol, t);
                col *= i.color;

                return col;
            }
            ENDHLSL
        }
    }
}
