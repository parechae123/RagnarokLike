Shader "Unlit/MaskedTexture"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}    // 표시할 주 텍스처
        _MaskTex ("Mask Texture", 2D) = "white" {}    // 마스킹할 텍스처
        _ColorTint ("Color Tint", Color) = (1,1,1,1)  // 색상 조정
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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
            sampler2D _MaskTex;
            float4 _MainTex_ST;
            float4 _MaskTex_ST;
            float4 _ColorTint;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {


                fixed4 col = tex2D(_MainTex, i.uv);

                // 그레이스케일 값 계산
                float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
                fixed4 grayColor = fixed4(grey, grey, grey, col.a); // 그레이스케일 색상
                fixed4 maskColor = tex2D(_MaskTex, i.uv);

                // 프로퍼티에서 파란색 추가
                fixed4 finalColor = grayColor + _ColorTint; // 그레이스케일과 blueColor 결합

                finalColor.a *= maskColor.a;

                return finalColor; // 최종 색상 반환
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}