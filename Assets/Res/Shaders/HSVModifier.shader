Shader "Custom/HSVModifier"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _hue ("Hue", Float) = 1 // 1 <> 7
        _saturation ("Saturation", Float) = 1
        _value ("Value", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {

            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _hue;
            float _saturation;
            float _value;
            
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

            float3 rgb2hsv(float3 c)
            {
              float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
              float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
              float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

              float d = q.x - min(q.w, q.y);
              float e = 1.0e-10;
              return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsv2rgb(float3 c)
            {
              c = float3(c.x, clamp(c.yz, 0.0, 1.0));
              float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
              float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
              return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
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
                float4 samp = tex2D(_MainTex, i.uv);
                float3 hsv = rgb2hsv(samp.rgb); // hue sat val
                
                float hue = hsv.x * (_hue%7); // 0 <> 7
                float sat = hsv.y * _saturation; // 0 <> 1
                float val = hsv.z * _value; // 0 <> 1
                
                hsv = float3(hue, sat, val);
                
                return float4(hsv2rgb(hsv), 1);
            }

            
            
            ENDCG
        }
    }
}