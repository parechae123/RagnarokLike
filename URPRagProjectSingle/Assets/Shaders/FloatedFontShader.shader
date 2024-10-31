Shader "Custom/TextParticles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //����ݬڬ�֬��Ӭ� ������ �� ����ݬҬ��� �� ��֬��ڬ� �ެ�ج֬� �Ҭ��� �ެ֬߬��� 10, �߬� �߬ڬܬѬ� �߬� �Ҭ�ݬ���
        _Cols ("Columns Count", Int) = 5
        _Rows ("Rows Count", Int) = 3
    }
    SubShader
    {            
        Tags { "RenderType"="Opaque" "PreviewType"="Plane" "Queue" = "Transparent+1"}
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float4 uv : TEXCOORD0;
                //���� ��Ѭެ�� �Ӭ֬ܬ���� �� customData
                float4 customData1 : TEXCOORD1;
                float4 customData2 : TEXCOORD2;
            };           

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 uv : TEXCOORD0;
                float4 customData1 : TEXCOORD1;
                float4 customData2 : TEXCOORD2;
            };
            
            uniform sampler2D _MainTex;
            uniform uint _Cols;
            uniform uint _Rows;
            
            v2f vert (appdata v)
            {
                v2f o;
                //�����֬ެ� �լݬڬ߬� ����Ҭ�֬߬ڬ� ��֬�֬լѬ֬��� �ڬެ֬߬߬� �� ����ݬ֬լ߬ڬ� ��Ѭ٬��լѬ� w-�ܬ���լڬ߬Ѭ�� �Ӭ֬ܬ����?
                //���Ѭ� ������ �Ӭ�֬Ԭ� ���ݬ��ڬ�� ���� �լݬڬ߬� �Ӭ߬���� ��֬۬լ֬��.
                //������Ѭ���߬� ���ݬ��ڬ�� ����Ѭ��� ��� �լ֬ݬ֬߬ڬ� �߬� 100.
                float textLength = ceil(fmod(v.customData2.w, 100));

                o.vertex = UnityObjectToClipPos(v.vertex);
                //����ݬ��Ѭ֬� ��Ѭ٬ެ֬� UV ��֬ܬ�����, �ڬ���լ� �ڬ� �ܬ��-�Ӭ� ������ �� ����ݬҬ���
                o.uv.xy = v.uv.xy * fixed2(textLength / _Cols, 1.0 / _Rows);
                o.uv.zw = v.uv.zw;
                o.color = v.color;                
                o.customData1 = floor(v.customData1);
                o.customData2 = floor(v.customData2);
                return o;
            }
            
            fixed4 frag (v2f v) : SV_Target
            {
                fixed2 uv = v.uv.xy;
                //���߬լ֬ܬ� ��ڬެӬ�ݬ� �� ����Ҭ�֬߬ڬ�
                uint ind = floor(uv.x * _Cols);

                uint x = 0;
                uint y = 0;

                //���߬լ֬ܬ� �ܬ���լڬ߬Ѭ�� �Ӭ֬ܬ����, ���լ֬�جѬ�ڬ� ����� ��ݬ֬ެ֬߬�
                //0-3 - customData1
                //4-7 - customData2
                uint dataInd = ind / 3;
                //����ݬ��Ѭ֬� �٬߬Ѭ�֬߬ڬ� �Ӭ�֬� 6 ��Ѭ٬��լ�� ���Ѭܬ�ӬѬ߬߬�� �� �߬�ج߬�� float
                uint sum = dataInd < 4 ? v.customData1[dataInd] : v.customData2[dataInd - 4];

                //���֬����֬լ��Ӭ֬߬߬� ��Ѭ��Ѭܬ�Ӭܬ� float �� ���ݬ��֬߬ڬ� �����ܬ� �� ����ݬҬ�� ��ڬެӬ�ݬ�
                for(int i = 0; i < 3; ++i)
                {
                    if (dataInd > 3 & i == 3) break;
                    //��ܬ��Ԭݬ�֬� �լ� �Ҭ�ݬ��֬Ԭ�, �ڬ߬Ѭ�� ���ݬ��ڬ� 10^2 = 99 �� ��.��.
                    uint val = ceil(pow(10, 5 - i * 2));
                    x = sum / val;
                    sum -= x * val;

                    val = ceil(pow(10, 4 - i * 2));
                    y = sum / val;
                    sum -= floor(y * val);

                    if (dataInd * 3 + i == ind) i = 3;
                }                

                float cols = 1.0 / _Cols;
                float rows = 1.0 / _Rows;
                //���լӬڬԬѬ֬� UV-�ܬ���լڬ߬Ѭ��, �ڬ���ݬ�٬�� �ܬ��-�Ӭ� ������, ����ݬҬ���, �ڬ߬լ֬ܬ�
                //�� �߬�ެ֬� �����ܬ� �� ����ݬҬ�� ��ݬ֬ެ֬߬��
                uv.x += x * cols - ind * rows;
                uv.y += y * rows;
                
                return tex2D(_MainTex, uv.xy) * v.color;
            }
            ENDCG
        }
    }
}