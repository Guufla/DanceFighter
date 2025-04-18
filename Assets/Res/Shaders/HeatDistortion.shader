Shader "Custom/HeatDistortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _strength_x("Distort Strength X", Float) = 1.0
        _strength_y("Distort Strength Y", Float) = 1.0
        _noiseTex("Noise Texture", 2D) = "white" {}
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
            float _strength_x;
            float _strength_y;
            sampler2D _noiseTex;
            

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

            v2f vert(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
                float2 sampCoord = i.uv;
                float2 noiseCoord = i.uv;
                float3 noise = tex2D(_noiseTex, i.uv);
                noiseCoord.x *= noise;
                sampCoord.x += noiseCoord.x;
                sampCoord.y += noiseCoord.y;

                float4 samp = tex2D(_MainTex, sampCoord);
                float4 color = float4(samp.rgb, 1);
                return color;
            }
            ENDCG
        }
    }
}