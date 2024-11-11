Shader "Custom/Billboard"
{
	Properties
	{
		// �ؽ�ó �Ӽ� ����: _MainTex�� 2D �ؽ�ó�� �⺻���� ���
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{
		// �±� ����: �����ϰ� �������ϱ� ���� ����
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }

		// ���� ���� �� ���� ����
		ZWrite Off // ���� ���ۿ� ���⸦ ��Ȱ��ȭ
		Blend SrcAlpha OneMinusSrcAlpha // ���� ���� ����

		Pass
		{
			CGPROGRAM
			#pragma vertex vert // ���� ���̴� ���
			#pragma fragment frag // �ȼ� ���̴� ���

			#include "UnityCG.cginc" // Unity�� �⺻ CG ���̺귯�� ����

			// ���ø����̼� ������ ���� ����: ���� ��ġ�� UV ��ǥ ����
			struct appdata
			{
				float4 pos : POSITION; // ���� ��ġ
				float2 uv : TEXCOORD0; // UV ��ǥ
			};

	// ���ؽ� ��� ���� ����: ��ȯ�� ���� ��ġ�� UV ��ǥ ����
		struct v2f
		{
			float4 pos : SV_POSITION; // ��ȯ�� ���� ��ġ
			float2 uv : TEXCOORD0; // UV ��ǥ
		};

		// 3D ������ ������ ��Ÿ���� ���� �ʱ�ȭ
		const float3 vect3Zero = float3(0.0, 0.0, 0.0);

		sampler2D _MainTex; // ���� �ؽ�ó ���÷� ����

	// ���� ���̴� �Լ�
	v2f vert(appdata v)
	{
		v2f o; // ��� ����ü �ʱ�ȭ

		// ī�޶� ��ġ�� �� �������� ��ȯ (3D ������ ��������)
		float4 camPos = float4(UnityObjectToViewPos(vect3Zero).xyz, 1.0);

		// ������ ���� ���͸� ��� (Z ��ǥ�� 0���� ����)
		float4 viewDir = float4(v.pos.x, v.pos.y, 0.0, 0.0);

		// ī�޶� ��ġ�� ������ ��ġ��, �̸� �������� ��Ʈ������ �����Ͽ� ���� ��ġ ���
		float4 outPos = mul(UNITY_MATRIX_P, camPos + viewDir);

		o.pos = outPos; // ��ȯ�� ��ġ�� ��� ����ü�� ����
		o.uv = v.uv; // UV ��ǥ�� ��� ����ü�� ����

		return o; // ��� ����ü ��ȯ
	}

	// �ȼ� ���̴� �Լ�
	fixed4 frag(v2f i) : SV_Target
	{
		// �ؽ�ó�� ���ø��Ͽ� ���� ���� ��ȯ
		return tex2D(_MainTex, i.uv);
	}
	ENDCG
}
	
	}
}
