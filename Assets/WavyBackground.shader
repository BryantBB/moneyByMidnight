Shader "Custom/EarthboundBackground"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        [Header(Tiling and Scrolling)]
        _Tiling ("Texture Tiling", Vector) = (5.0, 5.0, 0, 0)
        _ScrollSpeedX ("Scroll Speed X", Float) = 0.5
        _ScrollSpeedY ("Scroll Speed Y", Float) = 0.5
        
        [Header(Horizontal Wave Settings)]
        _WaveSpeedX ("Wave Time Speed", Float) = 5.0
        _WaveFreqX ("Wave Frequency (Density)", Float) = 10.0
        _WaveAmpX ("Wave Amplitude (Size)", Float) = 0.05
        
        [Header(Vertical Wave Settings)]
        _WaveSpeedY ("Wave Time Speed", Float) = 0.0
        _WaveFreqY ("Wave Frequency (Density)", Float) = 0.0
        _WaveAmpY ("Wave Amplitude (Size)", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "PreviewType"="Plane" }
        LOD 100

        Pass
        {
            CGPROGRAM
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Tiling;
            
            float _ScrollSpeedX;
            float _ScrollSpeedY;
            
            float _WaveSpeedX;
            float _WaveFreqX;
            float _WaveAmpX;
            
            float _WaveSpeedY;
            float _WaveFreqY;
            float _WaveAmpY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _Tiling.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float waveX = sin(uv.y * _WaveFreqX + _Time.y * _WaveSpeedX) * _WaveAmpX;
                float waveY = sin(uv.x * _WaveFreqY + _Time.y * _WaveSpeedY) * _WaveAmpY;

                uv.x += waveX;
                uv.y += waveY;

                uv.x += _Time.y * _ScrollSpeedX;
                uv.y += _Time.y * _ScrollSpeedY;

                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}