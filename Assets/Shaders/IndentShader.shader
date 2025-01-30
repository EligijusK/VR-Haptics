Shader "Custom/IndentShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _DisplacementMap ("Displacement Map", 2D) = "white" {}
        _DisplacementScale ("Displacement Scale", Range(0,0.1)) = 0.05
        _CutThreshold ("Cut Color Threshold", Range(0,1)) = 0.2
        _BloodColor ("Blood Color", Color) = (0.6, 0, 0, 1) // Darker red by default
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _DisplacementMap;
        float _DisplacementScale;
        float _CutThreshold;
        float4 _BloodColor;  // Now we have a color property we can set

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_DisplacementMap;
        };

        // Vertex function for displacement
        void vert (inout appdata_full v)
        {
            float4 disp = tex2Dlod(_DisplacementMap, float4(v.texcoord.xy, 0, 0));
            float displacement = disp.r;

            v.vertex.xyz -= v.normal * displacement * _DisplacementScale;
        }

        // Surface function for coloring
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 c = tex2D(_MainTex, IN.uv_MainTex);
            float4 disp = tex2D(_DisplacementMap, IN.uv_DisplacementMap);
            float displacement = disp.r;

            // Blend factor for red color
            float blendFactor = saturate((displacement - _CutThreshold) / (1.0 - _CutThreshold));

            // Use the user-defined _BloodColor
            float3 bloodColor = _BloodColor.rgb;

            // Blend from the original color to the blood color
            float3 finalColor = lerp(c.rgb, bloodColor, blendFactor);

            o.Albedo = finalColor;
            o.Metallic = 0.0;
            o.Smoothness = 0.5;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
