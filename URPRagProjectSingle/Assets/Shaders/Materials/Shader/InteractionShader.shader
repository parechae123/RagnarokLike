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
                float4 projPos : TEXCOORD1;   // Ŭ�� ���� ��ġ
                float2 uv : TEXCOORD0;       // UV ��ǥ
            };

            // ���ؽ� ���̴�
            //mesh�� ������ appdata�� �Ű������� �޾� v2f ����ü�� �����Ͽ� ������ ���
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.projPos = ComputeScreenPos(o.pos);  // ȭ�� ���� ��ǥ ���
                o.uv = v.uv;
                return o;
            }
            sampler2D _CameraDepthTexture;
            uniform float4 _Color;

            // �����׸�Ʈ ���̴�
            //���������� ��ȯ�ؾ� �� ���� �����͸� ���
            half4 frag(v2f i) : SV_Target
            {    
                float tempDepth = LinearEyeDepth(i.pos.z);
                if(tempDepth > 2.5f)
                {
                    return _Color;  // �⺻ ����
                    
                }
                else
                {
                    return half4(0,0,0,0);
                }
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
