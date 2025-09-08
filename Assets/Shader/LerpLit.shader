Shader "URP/LerpLit"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _SubTex ("Sub Texture", 2D) = "white" {}
        _Lerp("Lerp", Range(0, 1)) = 0
        [Normal] _Normal("Normal", 2D) = "bump" {}
        _NormalScale("Normal Scale", Float) = 1
        _OcclusionMap("Occlusion", 2D) = "white" {}
        _ColorTint("色调", Color) = (1,1,1,1)
        _SmoothAll("光滑度", Range(0, 1)) = 0
        _Metallic("金属度", Range(0, 1)) = 0
        _Occlusion("环境光", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}
        LOD 300

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }
            ZTest LEqual
            ZWrite On
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment

            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            // GPU Instancing
            #pragma multi_compile_instancing

            // 光照与阴影
            #include "Hlsl/BaseCore.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
            
            struct VertexInput
            {
                half4 position : POSITION;
                half4 texcoord : TEXCOORD0;
                half2 lightmapUV : TEXCOORD1;
                half3 normal : NORMAL;
                half4 tangent : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                half4 position : SV_POSITION;
                half2 uv : TEXCOORD0;
                half3 lightmap_or_sh : TEXCOORD1;
                half3 positionWS : TEXCOORD2;
                half3 normalWS : TEXCOORD3;
                half3 tangentWS : TEXCOORD4;
                half3 bitangentWS : TEXCOORD5;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            VertexOutput Vertex(VertexInput input)
            {
                VertexOutput output = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                half3 positionWS = TransformObjectToWorld(input.position.xyz);
                half4 positionCS = TransformWorldToHClip(positionWS);

                output.position = positionCS;
                output.uv = input.texcoord.xy;
                output.positionWS = positionWS;

                // 计算世界法线
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normal, input.tangent);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = normalInput.tangentWS;
                output.bitangentWS = normalInput.bitangentWS;

                // 环境光采样
                output.lightmap_or_sh = LightmapOrSh(normalInput.normalWS, input.lightmapUV);

                return output;
            }

            sampler2D _MainTex;
            half4 _MainTex_ST;

            sampler2D _SubTex;
            half4 _SubTex_ST;
            float _Lerp;

            sampler2D _Normal;
            half4 _Normal_ST;
            half _NormalScale;

            sampler2D _OcclusionMap;
            half4 _OcclusionMap_ST;

            half4 _ColorTint;
            half _SmoothAll;
            half _Metallic;
            half _Occlusion;

            half4 Fragment(VertexOutput input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // 基础纹理
                half4 color = tex2D(_MainTex, input.uv * _MainTex_ST.xy + _MainTex_ST.zw);
                half4 subColor = tex2D(_SubTex, input.uv * _SubTex_ST.xy + _SubTex_ST.zw);
                color = lerp(color, subColor, _Lerp);
                color.rgb *= _ColorTint.rgb;

                half4 normalTS = tex2D(_Normal, input.uv * _Normal_ST.xy + _Normal_ST.zw);
                normalTS.rgb = UnpackNormalScale(normalTS, _NormalScale);

                half4 occlusion = tex2D(_OcclusionMap, input.uv * _OcclusionMap_ST.xy + _OcclusionMap_ST.zw);

                // PBR光照
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.viewDirectionWS = SafeNormalize(GetCameraPositionWS() - input.positionWS);
                inputData.shadowCoord = WorldToShadowCoord(input.positionWS + float3(0, 1, 0));
                inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS, input.bitangentWS, input.normalWS));
                inputData.normalWS = normalize(inputData.normalWS);

                // 环境光颜色
                inputData.bakedGI = BakedGI(input.lightmap_or_sh, inputData.normalWS);

                SurfaceData surfaceData;
                surfaceData.albedo = color.rgb;
                surfaceData.specular = half3(1, 1, 1);
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _SmoothAll * color.a;
                surfaceData.normalTS = half3(0, 0, 1);
                surfaceData.emission = half3(0, 0, 0);
                surfaceData.occlusion = _Occlusion * occlusion.r;
                surfaceData.alpha = 1;
                surfaceData.clearCoatMask = 0;
                surfaceData.clearCoatSmoothness = 1;

                float4 ResultColor = UniversalFragmentPBR(inputData, surfaceData);

                return ResultColor;
            }

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{ "LightMode" = "ShadowCaster"}

            ZWrite On
            Cull Off

            HLSLPROGRAM

            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half4 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                half4 vertex : SV_POSITION;
                half2 uv : TEXCOORD0;
            };

            // 内置变量，由引擎自动赋值
            half3 _LightDirection;

            Varyings ShadowPassVertex(Attributes v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                half3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                half3 normalWS = TransformObjectToWorldDir(v.normal);

                half4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

            #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
            #else
                positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
            #endif

                Varyings o;
                o.vertex = positionCS;
                o.uv = v.texcoord.xy;
                return o;
            }

            half4 ShadowPassFragment(Varyings i) : SV_Target
            {
                return 0;
            }
            
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            Cull Off

            HLSLPROGRAM

            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #include "Hlsl/DepthPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMeta

            #include "Hlsl/MetaPass.hlsl"

            sampler2D _MainTex;
            half4 _MainTex_ST;

            half _SmoothAll;

            half4 UniversalFragmentMeta(Varyings input) : SV_Target
            {
                // 基础纹理
                half4 color = tex2D(_MainTex, input.uv * _MainTex_ST.xy + _MainTex_ST.zw);

                MetaInputData metaInput = (MetaInputData)0;
                metaInput.albedo = color.rgb;
                metaInput.smoothness = _SmoothAll;
                metaInput.alpha = 1;

                return MetaFragmentResult(metaInput);
            }

            ENDHLSL
        }
    }
}
