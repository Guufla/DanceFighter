Shader "Custom/GlitchAndVector"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _effect_color ("Effect Color", Color) = (0, 0, 0, 1)
        [HideInInspector] _input_vector ("Input Vector", Vector) = (0, 0, 0, 0) // x, y, z (if needed) are for the vector (w is not used)
        [HideInInspector] _do_effect ("Do Effect 0<>1", Int) = 0 // 0 <> 1 (false <> true)
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

            float4 _effect_color;
            vector _input_vector;
            int _do_effect;
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

                // apply colors
                float4 texColor = tex2D(_MainTex, i.uv) * i.color;
                float4 outColor = lerp(_effect_color, texColor, dist);
                //float4 outColor = lerp(0, finalOutlineColor, foundAlpha);
                outColor = lerp(outColor, texColor, texColor.a);

                
                // Make new shader with 'glitch' effect on hit, then leave outline base color alone

                return outColor;
            }
            ENDCG
        }
    }
}