Shader "SNEngine/Vignette"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _VignetteColor ("Vignette Color", Color) = (0,0,0,1)

        _Amount ("Amount", Range(0,1)) = 0
        _Softness ("Softness", Range(0.001,0.5)) = 0.25

        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _AlphaSplitEnabled;

            fixed4 _Color;
            fixed4 _VignetteColor;
            float _Amount;
            float _Softness;

            v2f vert (appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.texcoord;
                OUT.color = IN.color * _Color;

                #ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 c = tex2D(_MainTex, uv);
                #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                if (_AlphaSplitEnabled)
                    c.a = tex2D(_AlphaTex, uv).r;
                #endif
                return c;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.uv) * IN.color;

                // Центрированные UV
                float2 uv = IN.uv - 0.5;

                // Нормализованная дистанция (0 центр → 1 угол)
                float dist = length(uv) / 0.70710678;

                // start radius управляется Amount
                float start = 1.0 - _Amount;

                float vignette = smoothstep(
                    start,
                    start + _Softness,
                    dist
                );

                vignette *= _VignetteColor.a;

                c.rgb = lerp(c.rgb, _VignetteColor.rgb, vignette);
                c.rgb *= c.a;

                return c;
            }
        ENDCG
        }
    }
}
