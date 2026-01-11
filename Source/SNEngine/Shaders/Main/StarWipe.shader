Shader "SNEngine/StarWipeClassic"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Transition Color", Color) = (0,0,0,1)
        _Amount ("Amount", Range(0,1)) = 0
        _Sides ("Sides", Range(3,12)) = 5
        _Rotation ("Rotation", Range(0,360)) = 0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
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

            struct appdata_t { float4 vertex : POSITION; float4 color : COLOR; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; fixed4 color : COLOR; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            fixed4 _Color;
            float _Amount;
            float _Sides;
            float _Rotation;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif
                return OUT;
            }

            fixed4 SampleSpriteTexture(float2 uv)
            {
                return tex2D(_MainTex, uv);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 sprite = SampleSpriteTexture(IN.uv) * IN.color;
                float2 p = IN.uv - 0.5;
                float r = length(p);

                // угол + поворот
                float angle = atan2(p.y, p.x) + radians(_Rotation);
                angle = fmod(angle + UNITY_TWO_PI, UNITY_TWO_PI);

                float sector = UNITY_TWO_PI / _Sides;
                float local = fmod(angle, sector);

                // классическая звезда: вершина = 1, впадина = 0.5
                float t = abs(local - sector * 0.5) / (sector * 0.5);
                float starRadius = lerp(1.0, 0.5, t);

                float mask = step(r, starRadius * _Amount);

                fixed4 result = lerp(sprite, _Color, 1.0 - mask);
                result.rgb *= result.a;
                return result;
            }
            ENDCG
        }
    }
}
