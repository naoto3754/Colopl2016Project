// Shader created with Shader Forge v1.17 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.17;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9361,x:33410,y:32634,varname:node_9361,prsc:2|custl-3377-OUT;n:type:ShaderForge.SFN_Color,id:6463,x:32854,y:32691,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_6463,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:4404,x:32854,y:32872,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_4404,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2e64b66ee0d32404297b73ea3c18fe15,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5358,x:32854,y:33063,ptovrint:False,ptlb:Shadow Texture ,ptin:_ShadowTexture,varname:node_5358,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:db75b19ba73a74fe099a334419b228d6,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:3103,x:32697,y:33256,ptovrint:False,ptlb:ShadowWeight,ptin:_ShadowWeight,varname:node_3103,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:289,x:33051,y:32998,varname:node_289,prsc:2|A-4404-RGB,B-5358-RGB,T-3103-OUT;n:type:ShaderForge.SFN_Multiply,id:3377,x:33238,y:32875,varname:node_3377,prsc:2|A-6463-RGB,B-289-OUT;proporder:6463-4404-5358-3103;pass:END;sub:END;*/

Shader "Shader Forge/Paper" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Texture ("Texture", 2D) = "white" {}
        _ShadowTexture ("Shadow Texture ", 2D) = "white" {}
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
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform sampler2D _ShadowTexture; uniform float4 _ShadowTexture_ST;
            uniform float _ShadowWeight;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float4 _ShadowTexture_var = tex2D(_ShadowTexture,TRANSFORM_TEX(i.uv0, _ShadowTexture));
                float3 finalColor = (_Color.rgb*lerp(_Texture_var.rgb,_ShadowTexture_var.rgb,_ShadowWeight));
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
