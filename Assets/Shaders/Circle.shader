Shader "Custom/Circle" {
    Properties {
    	_MainColor ("Color", Color) = (1,1,1,1)
        _Border1 ("Border1", Float) = 0
        _Border2 ("Border2", Float) = 0
        _Border3 ("Border3", Float) = 0
        _Border4 ("Border4", Float) = 0
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

            uniform fixed4 _MainColor;
            uniform fixed _Border1;
            uniform fixed _Border2;
            uniform fixed _Border3;
            uniform fixed _Border4;

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
            	if( !((dist>_Border1 && dist < _Border2) || (dist > _Border3 && dist < _Border4)) )
            		discard;
                return _MainColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
