Shader "Custom/ValueStep"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ramp_tex ("Ramp Texture", 2D) = "white" {}
        _lower_bound ("Lower Bound", Float) = 0
        _upper_bound ("Upper Bound", Float) = 1
        _val_scale ("Value Scale", Float) = 1
    }

    SubShader
    {
        Tags
        {
            // does it need to be transparent?
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite off
        Cull off

        Pass
        {

            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _ramp_tex;
            float _lower_bound;
            float _upper_bound;
            float _val_scale;
            
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
                //float sat = hsv.y; // 0 <> 1
                float val = hsv.z * _val_scale; // (0 <> 1) * _val_scale
                float newVal = tex2D(_ramp_tex, val);
                hsv = float3(hsv.xy, newVal);
                
                int flag = ceil(val - _lower_bound) * ceil(_upper_bound - val); // 0 || 1
                float3 rgb = flag*hsv2rgb(hsv) + (1-flag)*float3(samp.rgb);

                float4 outCol = float4(rgb, samp.a);
                //return float4(val.xxx, samp.a);
                return outCol;
            }

            
            
            ENDCG
        }
    }
}