Shader "Custom/UI/CircularSliderFillDepth"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _MinDist ("Min Distance", Float) = 0.25
        _SliderValue ("Slider Value", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Packages/com.meta.xr.depthapi/Runtime/BiRP/EnvironmentOcclusionBiRP.cginc"

            // DepthAPI Environment Occlusion
            #pragma multi_compile _ HARD_OCCLUSION SOFT_OCCLUSION

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                META_DEPTH_VERTEX_OUTPUT(1) // the number should stand for the previous TEXCOORD# + 1

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO // required for stereo
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _MinDist;
            float _SliderValue;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); // required to support stereo
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // v.vertex (object space coordinate) might have a different name in your vert shader
                META_DEPTH_INITIALIZE_VERTEX_OUTPUT(o, v.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                // Calculate distance from the UV center (0.5, 0.5) to the current fragment
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);

                // Discard fragments closer than _MinDist to the center
                if(dist < _MinDist)
                {
                    discard;
                }

                // Calculate how far the value is from 0.5
                float distanceFromMiddle = abs(_SliderValue - 0.5) * 2; // Normalized to range [0, 1]
                // Linearly interpolate between green and red based on distance from the middle
                // Closer to 0.5 (green) if distanceFromMiddle is small, closer to ends (red) if large
                fixed3 colorChange = lerp(fixed3(0,1,0), fixed3(1,0,0), distanceFromMiddle);
                half4 col = tex2D(_MainTex, i.uv);
                col.rgb *= colorChange; // Apply the blended color
                
                col.a *= _Color.a; // Apply the alpha value from _Color to the output color
                
                META_DEPTH_OCCLUDE_OUTPUT_PREMULTIPLY(i, col, 0.0);
                return col;
            }
            ENDCG
        }
    }
}