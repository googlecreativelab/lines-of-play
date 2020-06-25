Shader "ARCore/EAP/Camera Color Ramp Shader"
{
    Properties
    {
        _ColorRamp("Color Ramp", 2D) = "white" {}
    }
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

            sampler2D _ColorRamp;

            // Vertex shader that scales the quad to full screen.
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = float4(v.vertex.x * 2.0f, v.vertex.y * 2.0f, 1.0f, 1.0f);
                o.uv = ArCoreGetDepthUv(v.uv);
                return o;
            }

            // This shader displays the depth buffer data as a color ramp overlay
            // for use in debugging.
            float4 frag(v2f i) : SV_Target
            {
                // Unpack depth texture value.
                float d = ArCoreGetDepthMeters(i.uv);

                // Zero means no raw data available, render black.
                if (d == 0.0f)
                {
                    return float4(0, 0, 0, 1);
                }

                // Use depth as an index into the color ramp texture.
                return tex2D(_ColorRamp, float2(d / 3.0f, 0.0f));
            }

            ENDCG
        }
    }
}
