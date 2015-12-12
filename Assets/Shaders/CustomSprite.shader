Shader "Sprites/Custom Deco"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _MainColor ("Tint", Color) = (1,1,1,1)
            _ForwardThreshold ("Forward Threshold", Range(0,1)) = 1
            _BackThreshold ("Back Threshold", Range(0.001,1)) = 0.001
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
                half2 texcoord  : TEXCOORD0;
            };

            fixed4 _MainColor;
            fixed _ForwardThreshold;
            fixed _BackThreshold;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _MainColor;
#ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
#endif
                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _AlphaSplitEnabled;

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);
                if (_AlphaSplitEnabled)
                    color.a = tex2D (_AlphaTex, uv).r;

                return color;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed2 remove_forward = step(IN.texcoord.r, _ForwardThreshold)*IN.texcoord;
                fixed step_double  = step(_BackThreshold, remove_forward);
                fixed2 remove_double = step_double*remove_forward;
                fixed4 c = SampleSpriteTexture (remove_double) * IN.color;
                c.a *= step_double;
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
