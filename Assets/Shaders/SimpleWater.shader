Shader "Gloopy/SimpleWater"
{
    Properties
    {
        _ShallowColor ("Shallow Color", Color) = (0.20, 0.65, 0.85, 1.0)
        _DeepColor ("Deep Color", Color) = (0.05, 0.20, 0.45, 1.0)
        _FoamColor ("Foam Color", Color) = (0.80, 0.95, 1.00, 1.0)
        _WaveScale ("Wave Scale", Float) = 1.6
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveStrength ("Wave Strength", Float) = 0.08
        _FoamThreshold ("Foam Threshold", Range(0, 1)) = 0.72
        _Alpha ("Alpha", Range(0, 1)) = 0.85
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float waveValue : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _ShallowColor;
                half4 _DeepColor;
                half4 _FoamColor;
                float _WaveScale;
                float _WaveSpeed;
                float _WaveStrength;
                float _FoamThreshold;
                float _Alpha;
            CBUFFER_END

            float GetWaveValue(float2 xz, float timeValue)
            {
                float waveA = sin(xz.x * _WaveScale + timeValue * _WaveSpeed);
                float waveB = cos(xz.y * (_WaveScale * 1.35) - timeValue * (_WaveSpeed * 0.8));
                float waveC = sin((xz.x + xz.y) * (_WaveScale * 0.7) + timeValue * (_WaveSpeed * 1.2));
                return (waveA + waveB + waveC) / 3.0;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float timeValue = _Time.y;
                float waveValue = GetWaveValue(positionWS.xz, timeValue);

                positionWS.y += waveValue * _WaveStrength;

                output.positionWS = positionWS;
                output.positionCS = TransformWorldToHClip(positionWS);
                output.waveValue = waveValue;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float wave01 = saturate(input.waveValue * 0.5 + 0.5);
                half3 waterColor = lerp(_DeepColor.rgb, _ShallowColor.rgb, wave01);

                float3 viewDirection = normalize(_WorldSpaceCameraPos - input.positionWS);
                float fresnel = pow(1.0 - saturate(dot(float3(0.0, 1.0, 0.0), viewDirection)), 3.0);
                float foam = smoothstep(_FoamThreshold, 1.0, wave01);

                waterColor = lerp(waterColor, _FoamColor.rgb, max(foam * 0.5, fresnel * 0.35));

                return half4(waterColor, _Alpha);
            }
            ENDHLSL
        }
    }
}
