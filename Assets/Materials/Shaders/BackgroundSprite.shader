Shader "Sprites/Background"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _MainColor ("Tint", Color) = (1,1,1,1)
        	_OffsetX ("Offset X", Range(0,1)) = 0
        	_OffsetY ("Offset Y", Range(0,1)) = 0
        	_TilingX ("Tiling X", Range(0,1)) = 1
        	_TilingY ("Tiling Y", Range(0,1)) = 1
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
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
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
            fixed _OffsetX;
         	fixed _OffsetY;
        	fixed _TilingX;
        	fixed _TilingY;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.texcoord = half2(IN.texcoord.x*_TilingX+_OffsetX, IN.texcoord.y*_TilingY+_OffsetY);
                OUT.color = IN.color * _MainColor;
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
            	fixed2 customUV = IN.texcoord;
                fixed4 c = SampleSpriteTexture (customUV) * IN.color;
                if(c.r == 0 && c.g == 0 && c.b == 0)
                	c.a = 0;
                return c;
            }
            ENDCG
        }
    }
}
