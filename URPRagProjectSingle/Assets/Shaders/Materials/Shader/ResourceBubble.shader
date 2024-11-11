Shader "Unlit/StereographicProjectionBubble"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PanSpeed ("Pan Speed", Float) = 0.1
        _Spherify ("Spherify", Range(0,1)) = 1
        _ColorTint ("Color Tint", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "PreviewType"="Plane" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _PanSpeed;
            float _Spherify;
            fixed4 _ColorTint;
            float _FillAmount;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 centered_uv = i.uv * 2.0 - 1.0;
                float z = sqrt(1.0 - saturate(dot(centered_uv.xy, centered_uv.xy)));
                float2 spherified_uv = centered_uv / (z + 1.0);
                float2 uv = spherified_uv * 0.5 + 0.5;

                uv = lerp(i.uv, uv, _Spherify);
                uv = TRANSFORM_TEX(uv, _MainTex);
                uv.x += frac(_Time.y * _PanSpeed);

                // 텍스처 색상에 ColorTint를 곱해 적용
                fixed4 col = tex2D(_MainTex, uv) * _ColorTint;

                // fillAmount에 따라 텍스처의 상단에서 하단으로 채워지는 방식 적용
                if (uv.y > _FillAmount)
                {
                    col.a = 0; // 위에서 아래로 채워지도록 설정
                }

                half sqrDist = dot(centered_uv.xy, centered_uv.xy);
                half mask = 1.0 - sqrDist;
                mask = saturate(mask / fwidth(mask));

                col.a *= mask;

                return col;
            }
            ENDCG
        }
    }
}
