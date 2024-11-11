Shader "Custom/TextParticles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // 이론적으로 행과 열의 수는 10보다 작을 수 있지만, 결코 더 클 수는 없음
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
                // customData를 포함하는 벡터
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
                // 메시지의 길이는 왜 w-좌표의 마지막 비트에 전달될까?
                // 이것이 셰이더 내에서 이 길이를 얻는 가장 쉬운 방법이기 때문.
                // 100으로 나눈 나머지만 얻으면 충분함.
                float textLength = ceil(fmod(v.customData2.w, 100));

                o.vertex = UnityObjectToClipPos(v.vertex);
                // 행과 열의 수에 기반하여 UV 텍스처의 크기를 얻음
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
                // 메시지 내의 문자 인덱스
                uint ind = floor(uv.x * _Cols);

                uint x = 0;
                uint y = 0;

                // 해당 요소를 포함하는 벡터의 인덱스
                // 0-3: customData1
                // 4-7: customData2
                uint dataInd = ind / 3;
                // 필요한 float에 포함된 모든 6자리의 값을 가져옴
                uint sum = dataInd < 4 ? v.customData1[dataInd] : v.customData2[dataInd - 4];

                // float를 풀어내어 문자 행과 열을 얻음
                for(int i = 0; i < 3; ++i)
                {
                    if (dataInd > 3 & i == 3) break;
                    // 올림 처리를 해야 함, 그렇지 않으면 10^2 = 99 등과 같은 오류 발생
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
                // 행, 열의 수, 인덱스, 요소의 행 및 열 번호를 사용하여 UV 좌표를 이동시킴
                uv.x += x * cols - ind * rows;
                uv.y += y * rows;
                
                return tex2D(_MainTex, uv.xy) * v.color;
            }
            ENDCG
        }
    }
}
