Shader "Unlit/MaskedTexture"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}    // ǥ���� �� �ؽ�ó
        _MaskTex ("Mask Texture", 2D) = "white" {}    // ����ŷ�� �ؽ�ó
        _ColorTint ("Color Tint", Color) = (1,1,1,1)  // ���� ����
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

                // �׷��̽����� �� ���
                float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
                fixed4 grayColor = fixed4(grey, grey, grey, col.a); // �׷��̽����� ����
                fixed4 maskColor = tex2D(_MaskTex, i.uv);

                // ������Ƽ���� �Ķ��� �߰�
                fixed4 finalColor = grayColor + _ColorTint; // �׷��̽����ϰ� blueColor ����

                finalColor.a *= maskColor.a;

                return finalColor; // ���� ���� ��ȯ
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}