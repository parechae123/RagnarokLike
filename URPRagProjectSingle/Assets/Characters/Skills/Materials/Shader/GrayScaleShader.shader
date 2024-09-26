Shader "Custom/MyShaderWithBlueGrayscale"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlueColor ("Blue Color", Color) = (0, 0, 1, 0) // 초기값을 파란색으로 설정
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // 텍스처 및 색상 프로퍼티
            sampler2D _MainTex;
            fixed4 _BlueColor; // blueColor 대신 _BlueColor 사용

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

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 텍스처에서 색상 샘플링
                fixed4 col = tex2D(_MainTex, i.uv);

                // 그레이스케일 값 계산
                float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
                fixed4 grayColor = fixed4(grey, grey, grey, col.a); // 그레이스케일 색상

                // 프로퍼티에서 파란색 추가
                fixed4 finalColor = grayColor + _BlueColor; // 그레이스케일과 blueColor 결합

                return finalColor; // 최종 색상 반환
            }
            ENDCG
        }
    }
}
