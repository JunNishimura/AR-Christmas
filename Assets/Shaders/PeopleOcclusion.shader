Shader "Custom/PeopleOcclusion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _BackgroundTex;
            sampler2D _DepthTex;
            sampler2D _StencilTex;

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 uv = i.uv;

                // flip x axis
                uv.x = 1.0 - uv.x;

                // correcting texture ratio that can be picked by ARHUmanBodyManager to the screen ratio
                float ratio = 1.62;
                uv.y /= ratio;
                uv.y += 1.0 - (ratio * 0.5);

                float stencil = tex2D(_StencilTex, uv).r;
                if (stencil < 0.9)
                {
                    return col;
                }

                float depth = tex2D(_DepthTex, uv).r;
                float sceneZ = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv));
                float delta = saturate(sceneZ - depth);
                if (delta > 0.0)
                {
                    return tex2D(_BackgroundTex, i.uv);
                }
                else 
                {
                    return col;
                }
            }
            ENDCG
        }
    }
}
