Shader "Custom/MyShaderWithBlueGrayscale"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlueColor ("Blue Color", Color) = (0, 0, 1, 0) // �ʱⰪ�� �Ķ������� ����
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

            // �ؽ�ó �� ���� ������Ƽ
            sampler2D _MainTex;
            fixed4 _BlueColor; // blueColor ��� _BlueColor ���

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
                // �ؽ�ó���� ���� ���ø�
                fixed4 col = tex2D(_MainTex, i.uv);

                // �׷��̽����� �� ���
                float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
                fixed4 grayColor = fixed4(grey, grey, grey, col.a); // �׷��̽����� ����

                // ������Ƽ���� �Ķ��� �߰�
                fixed4 finalColor = grayColor + _BlueColor; // �׷��̽����ϰ� blueColor ����

                return finalColor; // ���� ���� ��ȯ
            }
            ENDCG
        }
    }
}
