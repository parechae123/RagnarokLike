Shader "Custom/TextParticles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //¬¬¬à¬Ý¬Ú¬é¬Ö¬ã¬ä¬Ó¬à ¬ã¬ä¬â¬à¬Ü ¬Ú ¬ã¬ä¬à¬Ý¬Ò¬è¬à¬Ó ¬Ó ¬ä¬Ö¬à¬â¬Ú¬Ú ¬Þ¬à¬Ø¬Ö¬ä ¬Ò¬í¬ä¬î ¬Þ¬Ö¬ß¬î¬ê¬Ö 10, ¬ß¬à ¬ß¬Ú¬Ü¬Ñ¬Ü ¬ß¬Ö ¬Ò¬à¬Ý¬î¬ê¬Ö
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
                //¬´¬Ö ¬ã¬Ñ¬Þ¬í¬Ö ¬Ó¬Ö¬Ü¬ä¬à¬â¬Ñ ¬ã customData
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
                //¬±¬à¬é¬Ö¬Þ¬å ¬Õ¬Ý¬Ú¬ß¬Ñ ¬ã¬à¬à¬Ò¬ë¬Ö¬ß¬Ú¬ñ ¬á¬Ö¬â¬Ö¬Õ¬Ñ¬Ö¬ä¬ã¬ñ ¬Ú¬Þ¬Ö¬ß¬ß¬à ¬Ó ¬á¬à¬ã¬Ý¬Ö¬Õ¬ß¬Ú¬ç ¬â¬Ñ¬Ù¬â¬ñ¬Õ¬Ñ¬ç w-¬Ü¬à¬à¬â¬Õ¬Ú¬ß¬Ñ¬ä¬í ¬Ó¬Ö¬Ü¬ä¬à¬â¬Ñ?
                //¬´¬Ñ¬Ü ¬á¬â¬à¬ë¬Ö ¬Ó¬ã¬Ö¬Ô¬à ¬á¬à¬Ý¬å¬é¬Ú¬ä¬î ¬ï¬ä¬å ¬Õ¬Ý¬Ú¬ß¬å ¬Ó¬ß¬å¬ä¬â¬Ú ¬ê¬Ö¬Û¬Õ¬Ö¬â¬Ñ.
                //¬¥¬à¬ã¬ä¬Ñ¬ä¬à¬é¬ß¬à ¬á¬à¬Ý¬å¬é¬Ú¬ä¬î ¬à¬ã¬ä¬Ñ¬ä¬à¬Ü ¬à¬ä ¬Õ¬Ö¬Ý¬Ö¬ß¬Ú¬ñ ¬ß¬Ñ 100.
                float textLength = ceil(fmod(v.customData2.w, 100));

                o.vertex = UnityObjectToClipPos(v.vertex);
                //¬±¬à¬Ý¬å¬é¬Ñ¬Ö¬Þ ¬â¬Ñ¬Ù¬Þ¬Ö¬â UV ¬ä¬Ö¬Ü¬ã¬ä¬å¬â¬í, ¬Ú¬ã¬ç¬à¬Õ¬ñ ¬Ú¬Ù ¬Ü¬à¬Ý-¬Ó¬Ñ ¬ã¬ä¬â¬à¬Ü ¬Ú ¬ã¬ä¬à¬Ý¬Ò¬è¬à¬Ó
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
                //¬ª¬ß¬Õ¬Ö¬Ü¬ã ¬ã¬Ú¬Þ¬Ó¬à¬Ý¬Ñ ¬Ó ¬ã¬à¬à¬Ò¬ë¬Ö¬ß¬Ú¬Ú
                uint ind = floor(uv.x * _Cols);

                uint x = 0;
                uint y = 0;

                //¬ª¬ß¬Õ¬Ö¬Ü¬ã ¬Ü¬à¬à¬â¬Õ¬Ú¬ß¬Ñ¬ä¬í ¬Ó¬Ö¬Ü¬ä¬à¬â¬Ñ, ¬ã¬à¬Õ¬Ö¬â¬Ø¬Ñ¬ë¬Ú¬Û ¬ï¬ä¬à¬ä ¬ï¬Ý¬Ö¬Þ¬Ö¬ß¬ä
                //0-3 - customData1
                //4-7 - customData2
                uint dataInd = ind / 3;
                //¬±¬à¬Ý¬å¬é¬Ñ¬Ö¬Þ ¬Ù¬ß¬Ñ¬é¬Ö¬ß¬Ú¬Ö ¬Ó¬ã¬Ö¬ç 6 ¬â¬Ñ¬Ù¬â¬ñ¬Õ¬à¬Ó ¬å¬á¬Ñ¬Ü¬à¬Ó¬Ñ¬ß¬ß¬í¬ç ¬Ó ¬ß¬å¬Ø¬ß¬í¬Û float
                uint sum = dataInd < 4 ? v.customData1[dataInd] : v.customData2[dataInd - 4];

                //¬¯¬Ö¬á¬à¬ã¬â¬Ö¬Õ¬ã¬ä¬Ó¬Ö¬ß¬ß¬à ¬â¬Ñ¬ã¬á¬Ñ¬Ü¬à¬Ó¬Ü¬Ñ float ¬Ú ¬á¬à¬Ý¬å¬é¬Ö¬ß¬Ú¬Ö ¬ã¬ä¬â¬à¬Ü¬Ú ¬Ú ¬ã¬ä¬à¬Ý¬Ò¬è¬Ñ ¬ã¬Ú¬Þ¬Ó¬à¬Ý¬Ñ
                for(int i = 0; i < 3; ++i)
                {
                    if (dataInd > 3 & i == 3) break;
                    //¬à¬Ü¬â¬å¬Ô¬Ý¬ñ¬Ö¬Þ ¬Õ¬à ¬Ò¬à¬Ý¬î¬ê¬Ö¬Ô¬à, ¬Ú¬ß¬Ñ¬é¬Ö ¬á¬à¬Ý¬å¬é¬Ú¬Þ 10^2 = 99 ¬Ú ¬ä.¬Õ.
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
                //¬³¬Õ¬Ó¬Ú¬Ô¬Ñ¬Ö¬Þ UV-¬Ü¬à¬à¬â¬Õ¬Ú¬ß¬Ñ¬ä¬í, ¬Ú¬ã¬á¬à¬Ý¬î¬Ù¬å¬ñ ¬Ü¬à¬Ý-¬Ó¬à ¬ã¬ä¬â¬à¬Ü, ¬ã¬ä¬à¬Ý¬Ò¬è¬à¬Ó, ¬Ú¬ß¬Õ¬Ö¬Ü¬ã
                //¬Ú ¬ß¬à¬Þ¬Ö¬â ¬ã¬ä¬â¬à¬Ü¬Ú ¬Ú ¬ã¬ä¬à¬Ý¬Ò¬è¬Ñ ¬ï¬Ý¬Ö¬Þ¬Ö¬ß¬ä¬Ñ
                uv.x += x * cols - ind * rows;
                uv.y += y * rows;
                
                return tex2D(_MainTex, uv.xy) * v.color;
            }
            ENDCG
        }
    }
}