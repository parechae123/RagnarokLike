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
                // ���� �ؽ�ó�� ����ũ �ؽ�ó���� �ȼ��� ���ø�
                fixed4 mainColor = tex2D(_MainTex, i.uv) * _ColorTint;
                fixed4 maskColor = tex2D(_MaskTex, i.uv);

                // �׷��̽����� �� ���
                //float gray = dot(mainColor.rgb, float3(0.2989, 0.5870, 0.1140)); // �⺻���� �׷��̽����� ���
                
                //mainColor.rgb = gray.xxx * float3(_ColorTint.r, _ColorTint.g, _ColorTint.b);

                // ����ũ �ؽ�ó�� ���� ���� ����Ͽ� �� �ؽ�ó�� ����ŷ ����
                mainColor.a *= maskColor.a;

                return mainColor;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}