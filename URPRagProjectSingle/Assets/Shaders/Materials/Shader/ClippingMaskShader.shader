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
                // 메인 텍스처와 마스크 텍스처에서 픽셀을 샘플링
                fixed4 mainColor = tex2D(_MainTex, i.uv) * _ColorTint;
                fixed4 maskColor = tex2D(_MaskTex, i.uv);

                // 그레이스케일 값 계산
                //float gray = dot(mainColor.rgb, float3(0.2989, 0.5870, 0.1140)); // 기본적인 그레이스케일 계산
                
                //mainColor.rgb = gray.xxx * float3(_ColorTint.r, _ColorTint.g, _ColorTint.b);

                // 마스크 텍스처의 알파 값을 사용하여 주 텍스처에 마스킹 적용
                mainColor.a *= maskColor.a;

                return mainColor;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}