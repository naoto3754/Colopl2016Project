Shader "Hidden/CustomBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlurSize ("Blur Size", 2D) = "white" {}
	}
	CGINCLUDE

	#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	fixed _BlurSize;

	v2f vert (appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;
	uniform half4 _MainTex_TexelSize;
	static const fixed2 offsets[9] = { 
		fixed2(-1.414, 1.414), fixed2( 0, 2), fixed2( 1.414, 1.414),
		fixed2(-2, 0), fixed2( 0, 0), fixed2( 2, 0),
		fixed2(-1.414,-1.414), fixed2( 0,-2), fixed2( 1.414,-1.414)
	};
	static const fixed weights[9] = { 
		1,2,1,
		2,4,2,
		1,2,1
	};
	static const int len = 9;

	fixed4 frag (v2f i) : SV_Target
	{			
		fixed4 finalColor = fixed4(0,0,0,0);

		for(int n1 = 0; n1 < len; n1++) {
			fixed2 o1 = offsets[n1];
			fixed4 col = tex2D(_MainTex, i.uv+o1*_BlurSize*_MainTex_TexelSize.xy);
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
