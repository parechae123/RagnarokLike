Shader "Custom/InteractionShader"
{
 Properties
    {
        _Color ("Color", Color) = (1,1,1,1)  // ���� �� Material���� ���� �� ���� ����
        //C#�� �ڷ��� ������ ����� ����� �ٸ��� �������� ���� ���� ������ ���� �� (string��,�ڷ���) ������ �ۼ��Ѵ�
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            //���̴� ó���� �����ϴ� ����, �� ���� ���ķ� HLSL�� �����ϵ�
            Blend SrcAlpha OneMinusSrcAlpha     //���İ��� ���
            Cull Off        //���� �� ��������
            ZTest GEqual   // ���̰� �� ��� ����
            ZWrite On      // ���� ���ۿ� ���� �� ����

            CGPROGRAM
            //�Լ� ���� ����
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

            // ���ؽ� ���̴�
            //mesh�� ������ appdata�� �Ű������� �޾� v2f ����ü�� �����Ͽ� ������ ���
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                return o;
            }
            uniform float4 _Color;

            // �����׸�Ʈ ���̴�
            //���������� ��ȯ�ؾ� �� ���� �����͸� ���
            half4 frag(v2f i) : SV_Target
            {    
                return _Color;  // �⺻ ����
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
