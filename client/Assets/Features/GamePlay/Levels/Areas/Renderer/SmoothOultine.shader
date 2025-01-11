Shader "Custom/BlurOutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _BlurSize ("Blur Size", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

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
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _BlurSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                half4 outlineColor = _OutlineColor;

                float2 blurOffset = float2(_BlurSize / _ScreenParams.x, _BlurSize / _ScreenParams.y);
                half4 blurColor = tex2D(_MainTex, i.uv + blurOffset) +
                                  tex2D(_MainTex, i.uv - blurOffset) +
                                  tex2D(_MainTex, i.uv + float2(blurOffset.x, -blurOffset.y)) +
                                  tex2D(_MainTex, i.uv + float2(-blurOffset.x, blurOffset.y));

                blurColor /= 4.0;

                return lerp(color, outlineColor, blurColor.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}