Shader "LowPolyShaders/LowPolyPBRShader" {
	    Properties {
        _MainTex ("Color Scheme", 2D) = "white" {}
        _Tex2Dlod ("Tex2DlodSample", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            struct Attributes {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 tangentWS : TEXCOORD2;
                float3 bitangentWS : TEXCOORD3;
                float2 texcoord : TEXCOORD4;
                float4 color : COLOR;
            };

            float4 _Color;
            half _Glossiness;

            sampler2D _Tex2Dlod;
            float4    _Tex2Dlod_ST;
            Varyings vert (Attributes v) {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.vertex);
                o.worldPos = TransformObjectToWorld(v.vertex);
                o.normalWS = TransformObjectToWorldNormal(v.normal);
                o.tangentWS = TransformObjectToWorldDir(v.tangent.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * v.tangent.w;
                o.texcoord = v.texcoord;
                o.color = tex2Dlod(_Tex2Dlod, float4(v.texcoord.xy,0,0));
                return o;
            }

            half4 frag (Varyings IN) : SV_Target {
                
                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(IN.texcoord, surfaceData);
                surfaceData.albedo = IN.color.rgb;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Glossiness;

                half3 viewDir = SafeNormalize(GetCameraPositionWS() - IN.worldPos);
                half3 normalWS = normalize(IN.normalWS);
                Light mainLight = GetMainLight();
                half3 lightDir = normalize(mainLight.direction);
                half3 diffuse = surfaceData.albedo * mainLight.color.rgb * max(0.0, dot(normalWS, lightDir));

                half3 ambient = surfaceData.albedo * UNITY_LIGHTMODEL_AMBIENT.rgb;
                half3 finalColor = diffuse + ambient;

                return half4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}