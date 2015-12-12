
Shader "Shader Forge/Paper" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoShadowTex ("No Shadow Texture", 2D) = "white" {}
        _ShadowTex ("Shadow Texture ", 2D) = "white" {}
        _ShadowWeight ("ShadowWeight", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _NoShadowTex; uniform float4 _NoShadowTex_ST;
            uniform sampler2D _ShadowTex; uniform float4 _ShadowTex_ST;
            uniform float _ShadowWeight;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _Texture_var = tex2D(_NoShadowTex,TRANSFORM_TEX(i.uv0, _NoShadowTex));
                float4 _ShadowTexture_var = tex2D(_ShadowTex,TRANSFORM_TEX(i.uv0, _ShadowTex));
                float3 finalColor = _Color.rgb*lerp(_Texture_var.rgb, _ShadowTexture_var.rgb, _ShadowWeight);
                fixed4 finalRGBA = fixed4(finalColor,1);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
