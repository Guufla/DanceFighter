Shader "Custom/HealthBar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _color1 ("Color1", Color) = (1, 1, 1, 1)
        _fill ("Fill Value", Float) = 1 // 0 <> 1
        _angle ("Angle", Float) = -11.21
        _val1 ("Val1", Float) = 1
        _val2 ("Val2", Float) = 1
        _timescale ("Timescale", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            #define TAU 6.28318530718
            
            sampler2D _MainTex;
            float4 _color1;
            float4 _MainTex_ST;
            float _angle;
            float _fill;
            float _val1;
            float _val2;
            float _timescale;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // time, amplitude, and period
            float sawtooth_wave(float t,  float a, float p)
            {
                p = saturate(p);
                return 4*a/p*abs(((t-p/4)%p)-p/2)-a;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                int flagx = 1 - (1-(_angle*i.uv.x + i.uv.y)/(_angle+1) - (_fill+0.35)*0.6); // 0.21 <> 0.81 (Hard coded for specific healthbar texture)
                float2 coord = float2(i.uv.x, i.uv.y);
                float4 samp = tex2D(_MainTex, coord);
                
                float offset = sawtooth_wave( i.uv.y * TAU * _val2, 1, 1) * 0.01;
                float t = sawtooth_wave( (i.uv.x + offset - _Time.y * _timescale) * TAU * _val1, 1, 1) * 0.5 + 0.5;
                float4 c = flagx * lerp(samp, samp.a*float4(0, 0, 0, 1), t*1.5);
                return c * _color1;
            }
            ENDCG
        }
    }
}