Shader "Custom/Billboard"
{
	Properties
	{
		// 텍스처 속성 정의: _MainTex는 2D 텍스처로 기본값은 흰색
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{
		// 태그 설정: 투명하게 렌더링하기 위한 설정
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }

		// 깊이 쓰기 및 블렌딩 설정
		ZWrite Off // 깊이 버퍼에 쓰기를 비활성화
		Blend SrcAlpha OneMinusSrcAlpha // 알파 블렌딩 설정

		Pass
		{
			CGPROGRAM
			#pragma vertex vert // 정점 셰이더 사용
			#pragma fragment frag // 픽셀 셰이더 사용

			#include "UnityCG.cginc" // Unity의 기본 CG 라이브러리 포함

			// 애플리케이션 데이터 구조 정의: 정점 위치와 UV 좌표 포함
			struct appdata
			{
				float4 pos : POSITION; // 정점 위치
				float2 uv : TEXCOORD0; // UV 좌표
			};

	// 버텍스 출력 구조 정의: 변환된 정점 위치와 UV 좌표 포함
		struct v2f
		{
			float4 pos : SV_POSITION; // 변환된 정점 위치
			float2 uv : TEXCOORD0; // UV 좌표
		};

		// 3D 공간의 원점을 나타내는 벡터 초기화
		const float3 vect3Zero = float3(0.0, 0.0, 0.0);

		sampler2D _MainTex; // 메인 텍스처 샘플러 정의

	// 정점 셰이더 함수
	v2f vert(appdata v)
	{
		v2f o; // 출력 구조체 초기화

		// 카메라 위치를 뷰 공간으로 변환 (3D 공간의 원점에서)
		float4 camPos = float4(UnityObjectToViewPos(vect3Zero).xyz, 1.0);

		// 정점의 방향 벡터를 계산 (Z 좌표는 0으로 설정)
		float4 viewDir = float4(v.pos.x, v.pos.y, 0.0, 0.0);

		// 카메라 위치와 방향을 합치고, 이를 프로젝션 매트릭스를 적용하여 최종 위치 계산
		float4 outPos = mul(UNITY_MATRIX_P, camPos + viewDir);

		o.pos = outPos; // 변환된 위치를 출력 구조체에 저장
		o.uv = v.uv; // UV 좌표를 출력 구조체에 저장

		return o; // 출력 구조체 반환
	}

	// 픽셀 셰이더 함수
	fixed4 frag(v2f i) : SV_Target
	{
		// 텍스처를 샘플링하여 최종 색상 반환
		return tex2D(_MainTex, i.uv);
	}
	ENDCG
}
	
	}
}
