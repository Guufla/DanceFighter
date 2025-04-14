Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _samples ("Samples", Float) = 32.0
        _outline_size ("Outline Size", Float) = 1.0
        [HDR] _outline_color ("Outline Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags
        {
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

            float4 _outline_color;
            float _samples;
            float _outline_size;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float3 worldPosition : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz; // object space to world space conversion
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                // base outline
                float radius = _outline_size * 0.001;
                float angle = 0;
                float foundAlpha = 0;
                for (int k = 0; k < _samples; ++k)
                {
                    angle += _samples / 2 * UNITY_PI;
                    float2 testPoint = i.uv + float2(cos(angle) * radius * 0.13, sin(angle) * radius); // get point on uv
                    float sampledAlpha = tex2D(_MainTex, testPoint).a; // sample texture
                    foundAlpha = max(sampledAlpha, foundAlpha); // store most alpha we found
                }

                // apply colors
                //foundAlpha = floor(foundAlpha);
                float4 outColor = lerp(0, _outline_color, foundAlpha);
                float4 texColor = tex2D(_MainTex, i.uv) * i.color;
                outColor = lerp(outColor, texColor, texColor.a);
                
                return outColor;
            }
            ENDCG
        }
    }
}