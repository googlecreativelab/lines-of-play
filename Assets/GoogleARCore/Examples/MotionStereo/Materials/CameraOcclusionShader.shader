Shader "ARCore/EAP/Camera Occlusion Shader"
{
    SubShader
    {
        // No culling or depth
        Cull Off
        ZWrite On
        ZTest LEqual

        Tags { "Queue" = "Background+1" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "ARCoreDepth.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Vertex shader that scales the quad to full screen.
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = float4(v.vertex.x * 2.0f, v.vertex.y * 2.0f, 1.0f, 1.0f);
                o.uv = ArCoreGetDepthUv(v.uv);
                return o;
            }

            // This shader processes the depth texture data and outputs a depth-
            // only value. It is used to prepopulate the depth buffer in the scene.
            float frag(v2f i) : SV_Depth
            {
                // Unpack depth texture value.
                float z = ArCoreGetDepthMeters(i.uv);

                // Calculate appropriate depth buffer value.
                float near = _ProjectionParams.y;
                float far = _ProjectionParams.z;
                return (far * (z - near)) / (z * (far - near));
            }
            ENDCG
        }
    }
}
