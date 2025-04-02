Shader "Custom/PlayerEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _samples ("Samples", Float) = 32.0
        _outline_size ("Outline Size", Float) = 1.0
        _outline_color ("Outline Color", Color) = (0, 0, 0, 1)
        _effect_color ("Effect Color", Color) = (0, 0, 0, 1)
        [HideInInspector] _input_vector ("Input Vector", Vector) = (0, 0, 0, 0) // x, y, z (if needed) are for the vector (w is not used)
        [HideInInspector] _input_scale01 ("Input Scale 01", Float) = 0.0
        _dist_max ("Distance Max", Float) = 1.0
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
            float4 _effect_color;
            float _samples;
            float _outline_size;
            vector _input_vector;
            float _input_scale01;
            float _dist_max;

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
                // distance from input vector
                float dist = abs(length(i.worldPosition - _input_vector.xyz)); // do we need abs here?
                dist /= _dist_max; // normalize
                dist = clamp(dist, 0, 1);
                
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
                _input_scale01 = clamp(_input_scale01, 0, 1);
                _effect_color = _effect_color*_input_scale01 + _outline_color*(1 - _input_scale01);
                float4 finalOutlineColor = lerp(_effect_color, _outline_color, dist);
                float4 outColor = lerp(0, finalOutlineColor, foundAlpha);
                float4 texColor = tex2D(_MainTex, i.uv) * i.color;
                outColor = lerp(outColor, texColor, texColor.a);

                
                

                return outColor;
            }
            ENDCG
        }
    }
}