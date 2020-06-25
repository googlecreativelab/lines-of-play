//-----------------------------------------------------------------------
// <copyright file="GaussianBlur.shader" company="Google">
//
// Copyright 2020 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

Shader "Hidden/GaussianBlur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    uniform half4 _MainTex_TexelSize;
    half4 _MainTex_ST;

    uniform float _BlurSize;

    struct v2f_tap
    {
        float4 pos: SV_POSITION;
        half2 uv20: TEXCOORD0;
        half2 uv21: TEXCOORD1;
        half2 uv22: TEXCOORD2;
        half2 uv23: TEXCOORD3;
    };

    static const half kNumWeights = 8;
    static const half kWeights[7] =
    {
        0.0205, 0.0855, 0.232, 0.324, 0.232, 0.0855, 0.0205
    };

    struct v2f_withBlurCoords8
    {
        float4 pos: SV_POSITION;
        half4 uv: TEXCOORD0;
        half2 offs: TEXCOORD1;
    };

    half4 fragBlur8(v2f_withBlurCoords8 i): SV_Target
    {
        half2 uv = i.uv.xy;
        half2 netFilterWidth = i.offs;
        half2 coords = uv - netFilterWidth * 3.0;

        half4 color = 0;
        for (int l = 0; l < kNumWeights - 1; ++l)
        {
            half4 tap = tex2D(_MainTex,
            UnityStereoScreenSpaceUVAdjust(coords, _MainTex_ST));
            color += tap * kWeights[l];
            coords += netFilterWidth;
        }
        return color;
    }

    ENDCG

    SubShader
    {
        ZTest Always Cull Off ZWrite Off Blend Off

        // Pass #0 - Horizontal blur.
        Pass
        {
            CGPROGRAM

            #pragma vertex vertBlurVertical
            #pragma fragment fragBlur8

            v2f_withBlurCoords8 vertBlurVertical(appdata_img v)
            {
                v2f_withBlurCoords8 o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv = half4(v.texcoord.xy, 1, 1);
                o.offs = _MainTex_TexelSize.xy * half2(0.0, 1.0) * _BlurSize;

                return o;
            }

            ENDCG
        }

        // Pass #1 - Vertical blur.
        Pass
        {
            CGPROGRAM

            #pragma vertex vertBlurHorizontal
            #pragma fragment fragBlur8

            v2f_withBlurCoords8 vertBlurHorizontal(appdata_img v)
            {
                v2f_withBlurCoords8 o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv = half4(v.texcoord.xy, 1, 1);
                o.offs = _MainTex_TexelSize.xy * half2(1.0, 0.0) * _BlurSize;

                return o;
            }

            ENDCG
        }

    }

    FallBack Off
}
