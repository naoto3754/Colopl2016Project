Shader "Custom/CircleGradation" {
    Properties {
    	_MainTex ("Main Texture", 2D) = "white" {}
    	_MainColor ("Color", Color) = (1,1,1,1)
        _BottomColor ("Inner Color", Color) = (1,1,1,1)
        _TopColor ("Outer Color", Color) = (0.5,0.5,0.5,1)
    }
    SubShader {
        Tags {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
			Blend SrcAlpha OneMinusSrcAlpha 
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform fixed4 _MainColor;
            uniform fixed4 _BottomColor;
            uniform fixed4 _TopColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }

            float4 frag(VertexOutput i) : COLOR {
            	fixed u = abs(i.uv0.r-0.5);
            	fixed v = abs(i.uv0.g-0.5);
            	fixed dist = u*u+v*v;
                fixed4 finalColor = lerp(_BottomColor,_TopColor,dist*2);
                float4 tex = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                return finalColor*tex*_MainColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
