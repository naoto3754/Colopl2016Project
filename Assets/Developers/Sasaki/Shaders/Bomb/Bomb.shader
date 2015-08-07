// Shader created with Shader Forge v1.17 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.17;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:4013,x:34331,y:33075,varname:node_4013,prsc:2|diff-71-OUT,emission-9960-OUT,transm-2417-OUT,lwrap-71-OUT,alpha-8086-OUT;n:type:ShaderForge.SFN_TexCoord,id:1940,x:31902,y:32470,varname:node_1940,prsc:2,uv:0;n:type:ShaderForge.SFN_Lerp,id:6359,x:31678,y:32609,varname:node_6359,prsc:2|A-1507-OUT,B-8203-OUT,T-1893-OUT;n:type:ShaderForge.SFN_Slider,id:1893,x:31107,y:32897,ptovrint:False,ptlb:Lerp_t,ptin:_Lerp_t,varname:node_1893,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_RemapRange,id:8203,x:31495,y:32609,varname:node_8203,prsc:2,frmn:0,frmx:1,tomn:0,tomx:0.5|IN-6524-OUT;n:type:ShaderForge.SFN_Slider,id:6524,x:31107,y:32704,ptovrint:False,ptlb:Remap,ptin:_Remap,varname:node_6524,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Vector1,id:1507,x:31495,y:32804,varname:node_1507,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:7040,x:31678,y:32804,varname:node_7040,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:330,x:31902,y:32619,varname:node_330,prsc:2|A-7040-OUT,B-6359-OUT;n:type:ShaderForge.SFN_Add,id:8278,x:32105,y:32619,varname:node_8278,prsc:2|A-1940-UVOUT,B-330-OUT;n:type:ShaderForge.SFN_RemapRange,id:4711,x:31495,y:32919,varname:node_4711,prsc:2,frmn:0,frmx:1,tomn:0,tomx:0.25|IN-1893-OUT;n:type:ShaderForge.SFN_TexCoord,id:6448,x:31902,y:32797,varname:node_6448,prsc:2,uv:0;n:type:ShaderForge.SFN_Lerp,id:5104,x:31678,y:32936,varname:node_5104,prsc:2|A-1507-OUT,B-4711-OUT,T-1893-OUT;n:type:ShaderForge.SFN_Append,id:2709,x:31902,y:32946,varname:node_2709,prsc:2|A-7040-OUT,B-5104-OUT;n:type:ShaderForge.SFN_Add,id:8423,x:32100,y:32887,varname:node_8423,prsc:2|A-6448-UVOUT,B-2709-OUT;n:type:ShaderForge.SFN_Tex2d,id:5571,x:32278,y:32619,ptovrint:False,ptlb:tex,ptin:_tex,varname:node_5571,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4c191eebe35fc4717833ccd24fb0ce20,ntxv:0,isnm:False|UVIN-8278-OUT;n:type:ShaderForge.SFN_Tex2d,id:2680,x:32280,y:32887,ptovrint:False,ptlb:tex1,ptin:_tex1,varname:_tex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4c191eebe35fc4717833ccd24fb0ce20,ntxv:0,isnm:False|UVIN-8423-OUT;n:type:ShaderForge.SFN_Blend,id:4705,x:32457,y:32745,varname:node_4705,prsc:2,blmd:10,clmp:True|SRC-5571-R,DST-2680-R;n:type:ShaderForge.SFN_Parallax,id:8557,x:32632,y:32618,varname:node_8557,prsc:2|UVIN-8278-OUT,HEI-4705-OUT;n:type:ShaderForge.SFN_Parallax,id:8433,x:32639,y:32893,varname:node_8433,prsc:2|UVIN-8423-OUT,HEI-4705-OUT;n:type:ShaderForge.SFN_Tex2d,id:5744,x:32946,y:32990,ptovrint:False,ptlb:tex4,ptin:_tex4,varname:_tex_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4c191eebe35fc4717833ccd24fb0ce20,ntxv:0,isnm:False|UVIN-8433-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:7269,x:32946,y:32792,ptovrint:False,ptlb:tex3,ptin:_tex3,varname:_tex_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4c191eebe35fc4717833ccd24fb0ce20,ntxv:0,isnm:False|UVIN-8557-UVOUT;n:type:ShaderForge.SFN_Blend,id:71,x:33158,y:32665,varname:node_71,prsc:2,blmd:10,clmp:True|SRC-8784-RGB,DST-7269-RGB;n:type:ShaderForge.SFN_Tex2d,id:8784,x:32943,y:32577,ptovrint:False,ptlb:node_8784,ptin:_node_8784,varname:node_8784,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4c191eebe35fc4717833ccd24fb0ce20,ntxv:0,isnm:False|UVIN-8433-UVOUT;n:type:ShaderForge.SFN_OneMinus,id:5470,x:33178,y:32872,varname:node_5470,prsc:2|IN-7269-R;n:type:ShaderForge.SFN_OneMinus,id:9214,x:33178,y:33031,varname:node_9214,prsc:2|IN-5744-R;n:type:ShaderForge.SFN_Multiply,id:2417,x:33408,y:32858,varname:node_2417,prsc:2|A-4690-OUT,B-5470-OUT;n:type:ShaderForge.SFN_Slider,id:4690,x:33296,y:32774,ptovrint:False,ptlb:Transmission,ptin:_Transmission,varname:node_4690,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Blend,id:9601,x:33406,y:33086,varname:node_9601,prsc:2,blmd:10,clmp:True|SRC-5470-OUT,DST-9214-OUT;n:type:ShaderForge.SFN_Fresnel,id:67,x:32531,y:33347,varname:node_67,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:7105,x:32737,y:33347,varname:node_7105,prsc:2|IN-67-OUT;n:type:ShaderForge.SFN_Blend,id:312,x:32980,y:33347,varname:node_312,prsc:2,blmd:10,clmp:True|SRC-7105-OUT,DST-7105-OUT;n:type:ShaderForge.SFN_Blend,id:7138,x:33205,y:33475,varname:node_7138,prsc:2,blmd:0,clmp:True|SRC-8784-R,DST-7269-R;n:type:ShaderForge.SFN_Multiply,id:4985,x:33205,y:33347,varname:node_4985,prsc:2|A-312-OUT,B-9119-OUT;n:type:ShaderForge.SFN_Slider,id:9119,x:32823,y:33551,ptovrint:False,ptlb:opa,ptin:_opa,varname:node_9119,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Blend,id:3681,x:33425,y:33347,varname:node_3681,prsc:2,blmd:14,clmp:True|SRC-4985-OUT,DST-7138-OUT;n:type:ShaderForge.SFN_Multiply,id:8086,x:33696,y:33345,varname:node_8086,prsc:2|A-3681-OUT,B-8262-OUT;n:type:ShaderForge.SFN_Slider,id:573,x:33124,y:33672,ptovrint:False,ptlb:depth,ptin:_depth,varname:node_573,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:-1,max:5;n:type:ShaderForge.SFN_Vector3,id:1376,x:32741,y:33784,varname:node_1376,prsc:2,v1:0.9,v2:0.4,v3:0.2;n:type:ShaderForge.SFN_Slider,id:9998,x:32584,y:33910,ptovrint:False,ptlb:color,ptin:_color,varname:node_9998,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2564103,max:1;n:type:ShaderForge.SFN_Multiply,id:4360,x:32916,y:33784,varname:node_4360,prsc:2|A-1376-OUT,B-9998-OUT;n:type:ShaderForge.SFN_Multiply,id:1743,x:33118,y:33784,varname:node_1743,prsc:2|A-4360-OUT,B-9601-OUT;n:type:ShaderForge.SFN_Add,id:581,x:33286,y:33784,varname:node_581,prsc:2|A-1743-OUT,B-1743-OUT;n:type:ShaderForge.SFN_Blend,id:8487,x:33441,y:33784,varname:node_8487,prsc:2,blmd:10,clmp:True|SRC-581-OUT,DST-581-OUT;n:type:ShaderForge.SFN_RemapRange,id:9960,x:33625,y:33784,varname:node_9960,prsc:2,frmn:0,frmx:4,tomn:0,tomx:5|IN-8487-OUT;n:type:ShaderForge.SFN_DepthBlend,id:8262,x:33487,y:33593,varname:node_8262,prsc:2|DIST-573-OUT;proporder:1893-6524-5571-2680-7269-8784-4690-9119-573-5744-9998;pass:END;sub:END;*/

Shader "Shader Forge/Bomb" {
    Properties {
        _Lerp_t ("Lerp_t", Range(0, 2)) = 0
        _Remap ("Remap", Range(0, 2)) = 0
        _tex ("tex", 2D) = "white" {}
        _tex1 ("tex1", 2D) = "white" {}
        _tex3 ("tex3", 2D) = "white" {}
        _node_8784 ("node_8784", 2D) = "white" {}
        _Transmission ("Transmission", Range(0, 5)) = 0
        _opa ("opa", Range(0, 1)) = 1
        _depth ("depth", Range(-5, 5)) = -1
        _tex4 ("tex4", 2D) = "white" {}
        _color ("color", Range(0, 1)) = 0.2564103
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _CameraDepthTexture;
            uniform float _Lerp_t;
            uniform float _Remap;
            uniform sampler2D _tex; uniform float4 _tex_ST;
            uniform sampler2D _tex1; uniform float4 _tex1_ST;
            uniform sampler2D _tex4; uniform float4 _tex4_ST;
            uniform sampler2D _tex3; uniform float4 _tex3_ST;
            uniform sampler2D _node_8784; uniform float4 _node_8784_ST;
            uniform float _Transmission;
            uniform float _opa;
            uniform float _depth;
            uniform float _color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 projPos : TEXCOORD5;
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float node_7040 = 0.0;
                float node_1507 = 0.0;
                float2 node_8423 = (i.uv0+float2(node_7040,lerp(node_1507,(_Lerp_t*0.25+0.0),_Lerp_t)));
                float2 node_8278 = (i.uv0+float2(node_7040,lerp(node_1507,(_Remap*0.5+0.0),_Lerp_t)));
                float4 _tex_var = tex2D(_tex,TRANSFORM_TEX(node_8278, _tex));
                float4 _tex1_var = tex2D(_tex1,TRANSFORM_TEX(node_8423, _tex1));
                float node_4705 = saturate(( _tex1_var.r > 0.5 ? (1.0-(1.0-2.0*(_tex1_var.r-0.5))*(1.0-_tex_var.r)) : (2.0*_tex1_var.r*_tex_var.r) ));
                float2 node_8433 = (0.05*(node_4705 - 0.5)*mul(tangentTransform, viewDirection).xy + node_8423);
                float4 _node_8784_var = tex2D(_node_8784,TRANSFORM_TEX(node_8433.rg, _node_8784));
                float2 node_8557 = (0.05*(node_4705 - 0.5)*mul(tangentTransform, viewDirection).xy + node_8278);
                float4 _tex3_var = tex2D(_tex3,TRANSFORM_TEX(node_8557.rg, _tex3));
                float3 node_71 = saturate(( _tex3_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_tex3_var.rgb-0.5))*(1.0-_node_8784_var.rgb)) : (2.0*_tex3_var.rgb*_node_8784_var.rgb) ));
                float3 w = node_71*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float node_5470 = (1.0 - _tex3_var.r);
                float node_2417 = (_Transmission*node_5470);
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_2417,node_2417,node_2417);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = node_71;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float4 _tex4_var = tex2D(_tex4,TRANSFORM_TEX(node_8433.rg, _tex4));
                float3 node_1743 = ((float3(0.9,0.4,0.2)*_color)*saturate(( (1.0 - _tex4_var.r) > 0.5 ? (1.0-(1.0-2.0*((1.0 - _tex4_var.r)-0.5))*(1.0-node_5470)) : (2.0*(1.0 - _tex4_var.r)*node_5470) )));
                float3 node_581 = (node_1743+node_1743);
                float3 emissive = (saturate(( node_581 > 0.5 ? (1.0-(1.0-2.0*(node_581-0.5))*(1.0-node_581)) : (2.0*node_581*node_581) ))*1.25+0.0);
/// Final Color:
                float3 finalColor = diffuse + emissive;
                float node_7105 = (1.0 - (1.0-max(0,dot(normalDirection, viewDirection))));
                fixed4 finalRGBA = fixed4(finalColor,(saturate(( (saturate(( node_7105 > 0.5 ? (1.0-(1.0-2.0*(node_7105-0.5))*(1.0-node_7105)) : (2.0*node_7105*node_7105) ))*_opa) > 0.5 ? (saturate(min(_node_8784_var.r,_tex3_var.r)) + 2.0*(saturate(( node_7105 > 0.5 ? (1.0-(1.0-2.0*(node_7105-0.5))*(1.0-node_7105)) : (2.0*node_7105*node_7105) ))*_opa) -1.0) : (saturate(min(_node_8784_var.r,_tex3_var.r)) + 2.0*((saturate(( node_7105 > 0.5 ? (1.0-(1.0-2.0*(node_7105-0.5))*(1.0-node_7105)) : (2.0*node_7105*node_7105) ))*_opa)-0.5))))*saturate((sceneZ-partZ)/_depth)));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _CameraDepthTexture;
            uniform float _Lerp_t;
            uniform float _Remap;
            uniform sampler2D _tex; uniform float4 _tex_ST;
            uniform sampler2D _tex1; uniform float4 _tex1_ST;
            uniform sampler2D _tex4; uniform float4 _tex4_ST;
            uniform sampler2D _tex3; uniform float4 _tex3_ST;
            uniform sampler2D _node_8784; uniform float4 _node_8784_ST;
            uniform float _Transmission;
            uniform float _opa;
            uniform float _depth;
            uniform float _color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 projPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float node_7040 = 0.0;
                float node_1507 = 0.0;
                float2 node_8423 = (i.uv0+float2(node_7040,lerp(node_1507,(_Lerp_t*0.25+0.0),_Lerp_t)));
                float2 node_8278 = (i.uv0+float2(node_7040,lerp(node_1507,(_Remap*0.5+0.0),_Lerp_t)));
                float4 _tex_var = tex2D(_tex,TRANSFORM_TEX(node_8278, _tex));
                float4 _tex1_var = tex2D(_tex1,TRANSFORM_TEX(node_8423, _tex1));
                float node_4705 = saturate(( _tex1_var.r > 0.5 ? (1.0-(1.0-2.0*(_tex1_var.r-0.5))*(1.0-_tex_var.r)) : (2.0*_tex1_var.r*_tex_var.r) ));
                float2 node_8433 = (0.05*(node_4705 - 0.5)*mul(tangentTransform, viewDirection).xy + node_8423);
                float4 _node_8784_var = tex2D(_node_8784,TRANSFORM_TEX(node_8433.rg, _node_8784));
                float2 node_8557 = (0.05*(node_4705 - 0.5)*mul(tangentTransform, viewDirection).xy + node_8278);
                float4 _tex3_var = tex2D(_tex3,TRANSFORM_TEX(node_8557.rg, _tex3));
                float3 node_71 = saturate(( _tex3_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_tex3_var.rgb-0.5))*(1.0-_node_8784_var.rgb)) : (2.0*_tex3_var.rgb*_node_8784_var.rgb) ));
                float3 w = node_71*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float node_5470 = (1.0 - _tex3_var.r);
                float node_2417 = (_Transmission*node_5470);
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_2417,node_2417,node_2417);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float3 diffuseColor = node_71;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                float node_7105 = (1.0 - (1.0-max(0,dot(normalDirection, viewDirection))));
                return fixed4(finalColor * (saturate(( (saturate(( node_7105 > 0.5 ? (1.0-(1.0-2.0*(node_7105-0.5))*(1.0-node_7105)) : (2.0*node_7105*node_7105) ))*_opa) > 0.5 ? (saturate(min(_node_8784_var.r,_tex3_var.r)) + 2.0*(saturate(( node_7105 > 0.5 ? (1.0-(1.0-2.0*(node_7105-0.5))*(1.0-node_7105)) : (2.0*node_7105*node_7105) ))*_opa) -1.0) : (saturate(min(_node_8784_var.r,_tex3_var.r)) + 2.0*((saturate(( node_7105 > 0.5 ? (1.0-(1.0-2.0*(node_7105-0.5))*(1.0-node_7105)) : (2.0*node_7105*node_7105) ))*_opa)-0.5))))*saturate((sceneZ-partZ)/_depth)),0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
