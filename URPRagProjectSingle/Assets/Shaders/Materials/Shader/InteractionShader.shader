Shader "Custom/InteractionShader"
{
 Properties
    {
        _Color ("Color", Color) = (1,1,1,1)  // 엔진 내 Material에서 노출 될 값을 선언
        //C#의 자료형 변수명 방식의 선언과 다르게 변수명을 먼저 적고 엔진에 노출 될 (string값,자료형) 순으로 작성한다
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            //쉐이더 처리를 시작하는 구문, 이 구문 이후로 HLSL로 컴파일됨
            Blend SrcAlpha OneMinusSrcAlpha     //알파값을 사용
            Cull Off        //양쪽 다 렌더링함
            ZTest GEqual   // 깊이값 비교 방식 설정
            ZWrite On      // 깊이 버퍼에 깊이 값 저장

            CGPROGRAM
            //함수 실행 구문
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            // 버텍스 셰이더
            //mesh의 정보값 appdata를 매개변수로 받아 v2f 구조체에 대입하여 정점을 출력
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                return o;
            }
            uniform float4 _Color;

            // 프래그먼트 셰이더
            //최종적으로 반환해야 할 색상 데이터를 출력
            half4 frag(v2f i) : SV_Target
            {    
                return _Color;  // 기본 색상
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
