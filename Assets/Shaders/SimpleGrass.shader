Shader "Gloopy/SimpleGrass"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.32, 0.62, 0.24, 1.0)
        _DarkColor ("Dark Color", Color) = (0.18, 0.38, 0.14, 1.0)
        _HighlightColor ("Highlight Color", Color) = (0.58, 0.82, 0.36, 1.0)
        _PatchScale ("Patch Scale", Float) = 0.55
        _PatchStrength ("Patch Strength", Range(0, 1)) = 0.35
        _NoiseScale ("Noise Scale", Float) = 3.0
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.18
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _DarkColor;
                half4 _HighlightColor;
                float _PatchScale;
                float _PatchStrength;
                float _NoiseScale;
                float _NoiseStrength;
            CBUFFER_END

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 patchUV = floor(input.positionWS.xz * _PatchScale);
                float patchNoise = Hash21(patchUV);

                float detailNoise = Hash21(input.positionWS.xz * _NoiseScale);
                float combinedNoise = saturate(patchNoise * _PatchStrength + detailNoise * _NoiseStrength);

                half3 grassColor = lerp(_DarkColor.rgb, _BaseColor.rgb, patchNoise);
                grassColor = lerp(grassColor, _HighlightColor.rgb, combinedNoise);

                float3 normalWS = normalize(input.normalWS);
                float lightFromAbove = saturate(dot(normalWS, float3(0.0, 1.0, 0.0)));
                grassColor *= lerp(0.8, 1.1, lightFromAbove);

                return half4(grassColor, 1.0);
            }
            ENDHLSL
        }
    }
}
