#define ARCORE_DEPTH_SCALE 0.001        // mm to m
#define ARCORE_MAX_DEPTH_MM 8192.0
#define ARCORE_FLOAT_TO_5BITS 31        // (0.0, 1.0) -> (0, 31)
#define ARCORE_FLOAT_TO_6BITS 63        // (0.0, 1.0) -> (0, 63)
#define ARCORE_RGB565_RED_SHIFT 2048    // left shift 11 bits
#define ARCORE_RGB565_GREEN_SHIFT 32    // left shift 5 bits
#define ARCORE_BLEND_FADE_RANGE 0.01

sampler2D _CurrentDepthTexture;
uniform float4 _CurrentDepthTexture_TexelSize;
uniform float4 _UvTopLeftRight;
uniform float4 _UvBottomLeftRight;
uniform float _OcclusionBlendingScale;
uniform float _OcclusionOffsetMeters;

// Calculates depth texture UV given screen-space UV.
inline float2 ArCoreGetDepthUv(float2 uv)
{
    float2 uvTop = lerp(_UvTopLeftRight.xy, _UvTopLeftRight.zw, uv.x);
    float2 uvBottom = lerp(_UvBottomLeftRight.xy, _UvBottomLeftRight.zw, uv.x);
    return lerp(uvTop, uvBottom, uv.y);
}

// Returns depth value in meters for a given depth texture UV.
inline float ArCoreGetDepthMeters(float2 uv)
{
    // The depth texture uses TextureFormat.RGB565.
    float4 rawDepth = tex2Dlod(_CurrentDepthTexture, float4(uv, 0, 0));
    float depth = (rawDepth.r * ARCORE_FLOAT_TO_5BITS * ARCORE_RGB565_RED_SHIFT)
                + (rawDepth.g * ARCORE_FLOAT_TO_6BITS * ARCORE_RGB565_GREEN_SHIFT)
                + (rawDepth.b * ARCORE_FLOAT_TO_5BITS);
    depth = min(depth, ARCORE_MAX_DEPTH_MM);
    depth *= ARCORE_DEPTH_SCALE;
    return depth;
}

// Returns depth value in meters for a given position in world space.
inline float ArCoreGetFragmentDepthMeters(float3 worldPos)
{
    return -mul(UNITY_MATRIX_V, float4(worldPos, 1.0)).z;
}

inline float _ArCoreGetSampleAlpha(float2 uv, float3 virtualDepth)
{
    float realDepth = ArCoreGetDepthMeters(uv);
    float signedDiffMeters = realDepth - virtualDepth;
    return saturate(signedDiffMeters / ARCORE_BLEND_FADE_RANGE);
}

inline float _ArCoreGetBlendedAlpha(float2 uv, float3 virtualDepth, float2 stride)
{
   
    const float2 center_bias = float2(7.0/16.0, 7.0/16.0);

    float s;
    s  = _ArCoreGetSampleAlpha(uv + (float2(0.0/8.0, 0.0/1.0) - center_bias) * stride, virtualDepth) * 1.0/52.0;
    s += _ArCoreGetSampleAlpha(uv + (float2(1.0/8.0, 1.0/2.0) - center_bias) * stride, virtualDepth) * 8.0/52.0;
    s += _ArCoreGetSampleAlpha(uv + (float2(2.0/8.0, 1.0/4.0) - center_bias) * stride, virtualDepth) * 9.0/52.0;
    s += _ArCoreGetSampleAlpha(uv + (float2(3.0/8.0, 3.0/4.0) - center_bias) * stride, virtualDepth) * 8.0/52.0;
    s += _ArCoreGetSampleAlpha(uv + (float2(4.0/8.0, 1.0/8.0) - center_bias) * stride, virtualDepth) * 8.0/52.0;
    s += _ArCoreGetSampleAlpha(uv + (float2(5.0/8.0, 5.0/8.0) - center_bias) * stride, virtualDepth) * 9.0/52.0;
    s += _ArCoreGetSampleAlpha(uv + (float2(6.0/8.0, 3.0/8.0) - center_bias) * stride, virtualDepth) * 8.0/52.0;
    s += _ArCoreGetSampleAlpha(uv + (float2(7.0/8.0, 7.0/8.0) - center_bias) * stride, virtualDepth) * 1.0/52.0;
    return s;
}

// Returns an alpha to apply to occluded geometry that blends
// depth samples from nearby texels.
inline float ArCoreGetAlphaForBlendedOcclusion(float2 uv, float3 worldPos)
{
    float virtualDepth = ArCoreGetFragmentDepthMeters(worldPos) - _OcclusionOffsetMeters;
    float2 stride = _CurrentDepthTexture_TexelSize * _OcclusionBlendingScale;
    return _ArCoreGetBlendedAlpha(uv, virtualDepth, stride);
}
