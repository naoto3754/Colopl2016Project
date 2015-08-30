// Shader created with Shader Forge v1.17 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.17;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1873,x:33229,y:32719,varname:node_1873,prsc:2|emission-1749-OUT,alpha-603-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:32551,y:32729,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:True,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2828-OUT;n:type:ShaderForge.SFN_Multiply,id:1086,x:32812,y:32818,cmnt:RGB,varname:node_1086,prsc:2|A-4805-RGB,B-5983-RGB,C-5376-RGB,D-7811-OUT;n:type:ShaderForge.SFN_Color,id:5983,x:32551,y:32915,ptovrint:False,ptlb:Main Color,ptin:_MainColor,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_VertexColor,id:5376,x:32551,y:33079,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1749,x:33025,y:32818,cmnt:Premultiply Alpha,varname:node_1749,prsc:2|A-1086-OUT,B-603-OUT;n:type:ShaderForge.SFN_Multiply,id:603,x:32812,y:32992,cmnt:A,varname:node_603,prsc:2|A-4805-A,B-5983-A,C-5376-A;n:type:ShaderForge.SFN_LightAttenuation,id:8036,x:32057,y:33240,varname:node_8036,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5326,x:32229,y:33240,varname:node_5326,prsc:2|A-8036-OUT,B-1231-OUT;n:type:ShaderForge.SFN_Vector1,id:1231,x:32057,y:33381,varname:node_1231,prsc:2,v1:2;n:type:ShaderForge.SFN_Add,id:2148,x:32393,y:33240,varname:node_2148,prsc:2|A-5326-OUT,B-7580-OUT;n:type:ShaderForge.SFN_Vector1,id:7580,x:32229,y:33381,varname:node_7580,prsc:2,v1:-1;n:type:ShaderForge.SFN_Clamp01,id:7811,x:32551,y:33240,varname:node_7811,prsc:2|IN-2148-OUT;n:type:ShaderForge.SFN_Slider,id:2710,x:31312,y:32885,ptovrint:False,ptlb:Forward Threshold,ptin:_ForwardThreshold,varname:node_2710,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_TexCoord,id:2459,x:31469,y:32689,varname:node_2459,prsc:2,uv:0;n:type:ShaderForge.SFN_Step,id:1257,x:31675,y:32765,varname:node_1257,prsc:2|A-2459-U,B-2710-OUT;n:type:ShaderForge.SFN_Multiply,id:3476,x:31857,y:32766,varname:node_3476,prsc:2|A-1257-OUT,B-2459-UVOUT;n:type:ShaderForge.SFN_Slider,id:2188,x:31828,y:32687,ptovrint:False,ptlb:Back Threshold,ptin:_BackThreshold,varname:node_2188,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Step,id:798,x:32186,y:32729,varname:node_798,prsc:2|A-2188-OUT,B-9598-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9598,x:32027,y:32766,varname:node_9598,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3476-OUT;n:type:ShaderForge.SFN_Multiply,id:2828,x:32371,y:32729,varname:node_2828,prsc:2|A-798-OUT,B-3476-OUT;proporder:4805-5983-2710-2188;pass:END;sub:END;*/

Shader "Shader Forge/ShadowSprite" {
    Properties {
        [PerRendererData]_MainTex ("MainTex", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _ForwardThreshold ("Forward Threshold", Range(0, 1)) = 1
        _BackThreshold ("Back Threshold", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _MainColor;
            uniform float _ForwardThreshold;
            uniform float _BackThreshold;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
                float attenuation = 1;
////// Emissive:
                float2 node_3476 = (step(i.uv0.r,_ForwardThreshold)*i.uv0);
                float2 node_2828 = (step(_BackThreshold,node_3476.r)*node_3476);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_2828, _MainTex));
                float node_603 = (_MainTex_var.a*_MainColor.a*i.vertexColor.a); // A
                float3 emissive = ((_MainTex_var.rgb*_MainColor.rgb*i.vertexColor.rgb*saturate(((attenuation*2.0)+(-1.0))))*node_603);
                float3 finalColor = emissive;
                return fixed4(finalColor,node_603);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _MainColor;
            uniform float _ForwardThreshold;
            uniform float _BackThreshold;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(1,2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 finalColor = 0;
                float2 node_3476 = (step(i.uv0.r,_ForwardThreshold)*i.uv0);
                float2 node_2828 = (step(_BackThreshold,node_3476.r)*node_3476);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_2828, _MainTex));
                float node_603 = (_MainTex_var.a*_MainColor.a*i.vertexColor.a); // A
                return fixed4(finalColor * node_603,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}