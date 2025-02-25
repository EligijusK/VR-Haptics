Shader "Custom/SimpleColorShader"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Use Surface Shader with the Standard lighting model
        #pragma surface surf Standard
        #pragma target 3.0

        sampler2D _MainTex;
        float4 _TintColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        // Surface function
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Sample the base texture
            float4 texColor = tex2D(_MainTex, IN.uv_MainTex);

            // Multiply by our tint color
            float4 finalColor = texColor * _TintColor;

            // Assign to Albedo / Alpha
            o.Albedo = finalColor.rgb;
            o.Alpha = finalColor.a;

            // You can customize metallic, smoothness, etc. here if you like:
            o.Metallic = 0.0;
            o.Smoothness = 0.5;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
