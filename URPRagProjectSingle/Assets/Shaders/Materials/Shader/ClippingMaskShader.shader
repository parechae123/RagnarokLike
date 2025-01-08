Shader "Custom/ColorScale"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}        // ǥ���� �� �ؽ�ó
        _ColorTint ("Color Tint", Color) = (1, 1, 1, 1)   // ���� ����
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
        LOD 100

        // ���� ������ ���� (������ ����)
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // �ؽ�ó ���÷�
            sampler2D _MainTex;
            float4 _ColorTint;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // ���� ���̴�
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy; // �ؽ�ó UV ��ǥ
                return o;
            }

            // �ȼ� ���̴�
            fixed4 frag(v2f i) : SV_Target
            {
                // 1. ���� �ؽ�ó���� ���� ���ø�
                fixed4 col = tex2D(_MainTex, i.uv);

                // 2. �׷��̽����� �� ���
                float grey = dot(col.rgb, float3(0.299, 0.587, 0.114)); // ��� ��� (�׷��̽�����)
                fixed4 grayColor = fixed4(grey, grey, grey, col.a); // �׷��̽����� ����

                // 3. _ColorTint�� ����Ͽ� ���� ����
                fixed4 tintedColor = lerp(grayColor, grayColor * _ColorTint, _ColorTint.a);

                // 4. ����ũ �ؽ�ó���� ���� ���� ���ø�

                // 5. ����ũ�� ���� ������ ����ũ ó�� (����ũ ���İ� 0�̸� ���� ����, 1�̸� ������)

                return tintedColor; // ���� ���� ��ȯ
            }

            ENDCG
        }
    }

    Fallback "Diffuse"
}
