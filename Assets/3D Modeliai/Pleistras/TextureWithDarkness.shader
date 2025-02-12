Shader "Custom/TextureWithDarknessAndTransparency"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Allows selecting a texture
        _Tiling ("Tiling", Vector) = (1,1,0,0) // X and Y tiling
        _Offset ("Offset", Vector) = (0,0,0,0) // X and Y offset
        _Darkness ("Darkness", Range(0, 1)) = 0.0 // Controls darkness from 0 (normal) to 1 (black)
        _Alpha ("Transparency", Range(0, 1)) = 1.0 // Controls transparency (1 = opaque, 0 = fully transparent)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // Enable transparency blending
        ZWrite Off // Prevents depth writing for correct transparency rendering

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST; // Unity's default scaling/tiling property
            float4 _Tiling;
            float4 _Offset;
            float _Darkness;
            float _Alpha;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _Tiling.xy + _Offset.xy; // Apply custom tiling and offset
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= (1.0 - _Darkness); // Darken the color
                col.a *= _Alpha; // Apply transparency
                return col;
            }
            ENDCG
        }
    }
}
