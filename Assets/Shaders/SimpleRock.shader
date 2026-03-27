Shader "Gloopy/SimpleRock"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.44, 0.42, 0.38, 1.0)
        _DarkColor ("Dark Color", Color) = (0.24, 0.22, 0.20, 1.0)
        _HighlightColor ("Highlight Color", Color) = (0.62, 0.60, 0.55, 1.0)
        _StrataColor ("Strata Color", Color) = (0.36, 0.31, 0.26, 1.0)
        _NoiseScale ("Noise Scale", Float) = 2.4
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.28
        _StrataScale ("Strata Scale", Float) = 4.0
        _StrataStrength ("Strata Strength", Range(0, 1)) = 0.22
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
                half4 _StrataColor;
                float _NoiseScale;
                float _NoiseStrength;
                float _StrataScale;
                float _StrataStrength;
            CBUFFER_END

            float Hash21(float2 p)
            {
                p = frac(p * float2(234.34, 435.34));
                p += dot(p, p + 34.23);
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
                float baseNoise = Hash21(floor(input.positionWS.xz * _NoiseScale));
                float detailNoise = Hash21(input.positionWS.xz * (_NoiseScale * 2.1));
                float rockNoise = saturate(baseNoise * 0.7 + detailNoise * _NoiseStrength);

                float strata = sin(input.positionWS.y * _StrataScale + detailNoise * 2.0) * 0.5 + 0.5;
                strata = smoothstep(0.35, 0.85, strata) * _StrataStrength;

                half3 rockColor = lerp(_DarkColor.rgb, _BaseColor.rgb, rockNoise);
                rockColor = lerp(rockColor, _StrataColor.rgb, strata);

                float3 normalWS = normalize(input.normalWS);
                float topLight = saturate(dot(normalWS, float3(0.0, 1.0, 0.0)));
                float sideShade = 1.0 - topLight;

                rockColor = lerp(rockColor, _HighlightColor.rgb, topLight * 0.35);
                rockColor *= lerp(0.85, 1.0, 1.0 - sideShade * 0.25);

                return half4(rockColor, 1.0);
            }
            ENDHLSL
        }
    }
}
