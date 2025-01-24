// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
Shader "Custom/CutShaderWithMask"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _ScalpelPos ("Scalpel Position (World)", Vector) = (0,0,0,0)
        _CutRadius ("Cut Radius", Float) = 0.5
        _CutDepth ("Cut Depth", Float) = 0.05
        [Toggle]_UseRed ("Show Red Cut?", Float) = 0

        // NEW: A mask texture that will store where the mesh is 'cut' (white areas = cut)
        _CutMask ("Cut Mask (R=Cut)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard vertex:vert
        #include "UnityCG.cginc"

        // Properties
        float4 _Color;
        float4 _ScalpelPos;
        float _CutRadius;
        float _CutDepth;
        float _UseRed;

        // The texture that holds the "cut" mask
        sampler2D _CutMask;

        struct Input
        {
            float2 uv_MainTex;    // We'll read from _CutMask using the same UVs as the main texture
            float3 worldPos;      // We'll ask Unity to give us worldPos if we want distance checks 
        };

        // VERTEX FUNCTION:
        // This modifies vertex positions (indenting them) based on the "cut mask."
        void vert (inout appdata_full v)
        {
            // 1. Calculate the vertex position in world space
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

            // 2. Calculate the normal in world space
            float3 normalDir = mul((float3x3)unity_ObjectToWorld, v.normal);
            normalDir = normalize(normalDir);

            // 3. Sample the cut-mask using the mesh UV
            //    If the mask is white (1.0), that means the vertex is "cut."
            float2 uv = v.texcoord.xy;
            float maskValue = tex2Dlod(_CutMask, float4(uv, 0, 0)).r; 
            // tex2Dlod is used inside vertex shaders to do a proper lod sample

            // 4. If this vertex is "marked as cut," indent it.
            //    maskValue of 1.0 => fully cut, 0.0 => not cut.
            //    You could scale indentation by maskValue.
            if (maskValue > 0.0)
            {
                // Push the vertex down along its normal
                v.vertex.xyz -= normalDir * (_CutDepth * maskValue);
            }
        }

        // SURFACE (FRAGMENT) FUNCTION:
        // This sets the final color, using the cut mask to color red if desired.
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Base color from _Color
            float3 baseColor = _Color.rgb;

            // Read the mask at this pixel
            float maskValue = tex2D(_CutMask, IN.uv_MainTex).r;

            // If we want to show the red cut and the mask indicates it's cut:
            if (_UseRed > 0.5 && maskValue > 0.0)
            {
                // Color it red (or blend to red)
                baseColor = float3(1.0, 0.0, 0.0);
            }

            o.Albedo = baseColor;
            o.Metallic = 0.0;
            o.Smoothness = 0.2;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
