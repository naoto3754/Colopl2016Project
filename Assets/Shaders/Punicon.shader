// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Punicon" {
Properties {
	
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};
			
			v2f vert (appdata_t v)
			{
				v2f o;
				fixed4 model = mul(UNITY_MATRIX_MV, v.vertex);

				model.x *= model.y;

				o.vertex = mul(UNITY_MATRIX_P, model);

				o.texcoord = v.texcoord;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed u = abs(i.texcoord.r-0.5);
				fixed v = abs(i.texcoord.g-0.5);
				fixed dist = max(u, v);
				fixed4 col = fixed4(fixed3(1,1,1),pow(dist*1.5,4));
				return col;
			}
		ENDCG
	}
}

}
