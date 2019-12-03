Shader "Custom/GlowShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glow ("Intensity", Range(0, 3)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" "IgnoreProjector" = "True"}
        LOD 200
        Cull Off
        ZWrite On 
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            CGPROGRAM
            
            #pragma target 3.0
            #pragma vertex vert 
            #pragma fragment frag 

            sampler2D _MainTex;
            half4 _MainTex_ST;
            fixed4 _Color;
            half _Glossiness;
            half _Metallic;
            half _Glow;

            struct appdata 
            {
                float4 pos : POSITION;
                half2 tex : TEXCOORD0;
            };

            struct v2f 
            {
                float4 pos : SV_POSITION;
                half2 tex : TEXCOORD0;
            };

            v2f vert (appdata v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.tex = v.tex * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            fixed4 frag (v2f f) : SV_Target 
            {
                 fixed4 col = tex2D(_MainTex, f.tex);
                 col *= _Color;
                 col *= _Glow;
                 return col;
            }
            ENDCG
        }
    }
}
