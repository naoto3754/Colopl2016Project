Shader "Hidden/CustomBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlurSize ("BlurSize", Range(0, 10)) = 0
	}
	CGINCLUDE

	#include "UnityCG.cginc"

	struct appdata
	{
		fixed4 vertex : POSITION;
		fixed2 uv : TEXCOORD0;
	};

	struct v2f
	{
		fixed2 uv : TEXCOORD0;
		fixed4 vertex : SV_POSITION;
	};


	v2f vert (appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.uv;
		return o;
	}

	float _BlurSize;
	sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	static const float2 offsets[9] = { 
		float2(-1.414, 1.414), float2( 0, 2), float2( 1.414, 1.414),
		float2(-2, 0), float2( 0, 0), float2( 2, 0),
		float2(-1.414,-1.414), float2( 0,-2), float2( 1.414,-1.414)
	};
	static const float weights[9] = { 
		1,2,1,
		2,4,2,
		1,2,1
	};
	static const int len = 9;

	fixed4 frag (v2f i) : SV_Target
	{			
		float4 finalColor = float4(0,0,0,0);

		for(int n1 = 0; n1 < len; n1++) {
			float2 o1 = offsets[n1];
			float4 col = tex2D(_MainTex, i.uv+o1*_BlurSize*_MainTex_TexelSize.xy);
			finalColor += col*weights[n1];
		}
		finalColor /= 16;
		finalColor += _BlurSize*0.0015;

		return finalColor;
	}
	ENDCG

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}

	}
}
