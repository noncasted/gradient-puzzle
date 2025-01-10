Shader "Custom/BlurredLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BlurRadius ("Blur Radius", Range(0.0, 1.0)) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _BlurRadius;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Fixed-size offsets for sampling
                float2 blurOffsets[5] = {
                    float2(-_BlurRadius, 0),
                    float2(-_BlurRadius * 0.5, 0),
                    float2(0, 0),
                    float2(_BlurRadius * 0.5, 0),
                    float2(_BlurRadius, 0)
                };

                // Corresponding weights for Gaussian blur
                float weights[5] = {0.1, 0.2, 0.4, 0.2, 0.1};

                float4 color = float4(0, 0, 0, 0);
                for (int j = 0; j < 5; j++)
                {
                    float2 sampleUV = i.uv + blurOffsets[j];
                    color += tex2D(_MainTex, sampleUV) * weights[j];
                }

                color *= _Color;
                return float4(color.rgb, color.a);
            }
            ENDCG
        }
    }
}