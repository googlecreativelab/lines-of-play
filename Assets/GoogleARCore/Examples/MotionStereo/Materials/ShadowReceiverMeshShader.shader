Shader "ARCore/EAP/Shadow Receiver Mesh Shader"
{
    Properties
    {
        // Sets the transparency of the applied shadow. This is in addition
        // to any existing shadow intensity settings set for each source.
        _GlobalShadowIntensity("Global Shadow Intensity", Range(0, 1)) = 0.6
        _MinimumMeshDistance("Minimum Mesh Distance", Range(0, 1000)) = 0
        _MaximumMeshDistance("Maximum Mesh Distance", Range(0, 1000)) = 1000
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Background+1"
            "IgnoreProjector" = "True"
            "RenderType" = "TransparentCutout"
        }

        LOD 200
        ZWrite Off
        Blend Zero SrcColor

        CGPROGRAM
        // Append 'addshadow' after 'alphatest:_Cutoff' to allow the Shadow
        // Receiver Mesh to cast its own shadow. Also in the
        // 'ShadowReceiverMesh' prefab, set 'Cast Shadows' to 'On' for the
        // 'Mesh Renderer' component.
        #pragma surface surf ShadowOnly vertex:vert alphatest:_Cutoff

        #pragma target 3.0

        #include "ARCoreDepth.cginc"

        float _GlobalShadowIntensity;
        float _MinimumMeshDistance;
        float _MaximumMeshDistance;
        
        float _FocalLengthX;
        float _FocalLengthY;
        float _PrincipalPointX;
        float _PrincipalPointY;
        int _ImageDimensionsX;
        int _ImageDimensionsY;
        float4x4 _ScreenRotation;
        
        struct Input
        {
            float depth;
        };

        struct VInput
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            uint id : SV_VertexID;
        };

        float4 GetVertex(float tex_x, float tex_y, float z)
        {
            float4 vertex = 0;

            if (z > 0)
            {
                float x = (tex_x - _PrincipalPointX) * z / _FocalLengthX;
                float y = (tex_y - _PrincipalPointY) * z / _FocalLengthY;
                vertex = float4(x, -y, z, 1);
            }
            
            vertex = mul(vertex, _ScreenRotation);
            
            return vertex;
        }

        void vert(inout VInput v, out Input OUT)
        {
            UNITY_INITIALIZE_OUTPUT(Input, OUT);

            float2 texID = int3((uint)v.id % (uint)_ImageDimensionsX,
                (uint)v.id / (uint)_ImageDimensionsX, 0);
            float2 depthTexuv = float2(texID.x / (float)_ImageDimensionsX,
                texID.y / (float)_ImageDimensionsY);
            OUT.depth = ArCoreGetDepthMeters(depthTexuv);

            float4 vertex = GetVertex(texID.x, texID.y, OUT.depth);
            v.vertex = vertex;
            v.normal = float3(0, 0, -1);
        }

        inline fixed4 LightingShadowOnly(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 c;
            c.rgb = lerp(s.Albedo, s.Albedo * atten, _GlobalShadowIntensity);
            c.a = s.Alpha;
            return c;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Constrains the shadow & occlusion mesh to a min and max depth.
            clip(_MaximumMeshDistance - IN.depth);
            clip(IN.depth - _MinimumMeshDistance);
            
            o.Albedo = 1;
            o.Alpha = 1;
        }

        ENDCG
    }

    Fallback "Transparent/Cutout/VertexLit"
}
