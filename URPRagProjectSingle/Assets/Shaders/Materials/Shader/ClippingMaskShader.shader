Shader "Custom/ColorScale"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}        // 표시할 주 텍스처
        _ColorTint ("Color Tint", Color) = (1, 1, 1, 1)   // 색상 조정
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
        LOD 100

        // 알파 블렌딩 설정 (투명도 적용)
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // 텍스처 샘플러
            sampler2D _MainTex;
            float4 _ColorTint;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // 정점 셰이더
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy; // 텍스처 UV 좌표
                return o;
            }

            // 픽셀 셰이더
            fixed4 frag(v2f i) : SV_Target
            {
                // 1. 메인 텍스처에서 색상 샘플링
                fixed4 col = tex2D(_MainTex, i.uv);

                // 2. 그레이스케일 값 계산
                float grey = dot(col.rgb, float3(0.299, 0.587, 0.114)); // 밝기 계산 (그레이스케일)
                fixed4 grayColor = fixed4(grey, grey, grey, col.a); // 그레이스케일 색상

                // 3. _ColorTint를 사용하여 색상 조정
                fixed4 tintedColor = lerp(grayColor, grayColor * _ColorTint, _ColorTint.a);

                // 4. 마스크 텍스처에서 알파 값을 샘플링

                // 5. 마스크의 알파 값으로 마스크 처리 (마스크 알파가 0이면 완전 투명, 1이면 불투명)

                return tintedColor; // 최종 색상 반환
            }

            ENDCG
        }
    }

    Fallback "Diffuse"
}
