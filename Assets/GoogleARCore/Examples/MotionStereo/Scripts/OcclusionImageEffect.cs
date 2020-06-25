//-----------------------------------------------------------------------
// <copyright file="OcclusionImageEffect.cs" company="Google">
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
using GoogleARCoreInternal;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// A component that controls the full-screen occlusion effect.
/// Exposes parameters to control the occlusion effect, which get applied every time update
/// gets called.
/// </summary>
[RequireComponent(typeof(Camera))]
public class OcclusionImageEffect : MonoBehaviour
{
    /// <summary>
    /// The image effect shader to blit every frame with.
    /// </summary>
    public Shader OcclusionShader;

    /// <summary>
    /// An optional blur shader to apply to occluded regions for the feathering effect.
    /// </summary>
    public Shader BlurShader;

    /// <summary>
    /// The blur kernel size applied to the camera feed. In pixels.
    /// </summary>
    [Space]
    public float BlurSize = 20f;

    /// <summary>
    /// The number of times occlusion map is downsampled before blurring. Useful for performance
    /// optimization. The value of 1 means no downsampling, each next one downsamples by 2.
    /// </summary>
    public int BlurDownsample = 2;

    /// <summary>
    /// Maximum occlusion transparency. The value of 1.0 means completely invisible when occluded.
    /// </summary>
    [Range(0, 1)]
    public float OcclusionTransparency = 1.0f;

    /// <summary>
    /// The bias added to the estimated depth. Useful to avoid occlusion of objects anchored to
    /// planes. In meters.
    /// </summary>
    [Space]
    public float OcclusionOffset = 0.08f;

    /// <summary>
    /// Controls whether occlusions are enabled or disabled. Occlusion transparency will be
    /// interpolated over a time period.
    /// </summary>
    public bool OcclusionEnabled = true;

    /// <summary>
    /// Velocity occlusions effect fades in/out when being enabled/disabled.
    /// </summary>
    public float OcclusionFadeVelocity = 4.0f;

    /// <summary>
    /// Instead of a hard z-buffer test, allows the asset to fade into the
    /// background along a range centered on the background depth.
    /// </summary>
    public float TransitionSizeMeters = 0.05f;

    private static readonly string k_CurrentDepthTexturePropertyName = "_CurrentDepthTexture";

    private Camera m_Camera;
    private Material m_OcclusionMaterial;
    private Material m_BlurMaterial;
    private bool m_SessionEnabled;
    private CommandBuffer m_CommandBuffer;
    private float m_CurrentOcclusionTransparency = 1.0f;

    /// <summary>
    /// Set the depth texture to use, useful for debugging.
    /// </summary>
    /// <param name="texture">The texture to setup.</param>
    public void SetDepthTexture(RenderTexture texture)
    {
        var attachDepthTexture = FindObjectOfType<DepthTextureController>();
        Debug.Assert(attachDepthTexture != null,
                     "DepthTextureController must be present in the scene.");
        attachDepthTexture.Materials.Remove(m_OcclusionMaterial);

        m_OcclusionMaterial.SetTexture(k_CurrentDepthTexturePropertyName, texture);
    }

    private void Start()
    {
        m_CurrentOcclusionTransparency = OcclusionTransparency;

        Debug.Assert(OcclusionShader != null, "Occlusion Shader parameter must be set.");
        m_OcclusionMaterial = new Material(OcclusionShader);
        m_OcclusionMaterial.SetFloat("_OcclusionTransparency", m_CurrentOcclusionTransparency);
        m_OcclusionMaterial.SetFloat("_OcclusionOffsetMeters", OcclusionOffset);
        m_OcclusionMaterial.SetFloat("_TransitionSizeMeters", TransitionSizeMeters);

        if (BlurShader != null)
        {
            m_BlurMaterial = new Material(BlurShader);
        }

        m_Camera = GetComponent<Camera>();
        m_Camera.depthTextureMode |= DepthTextureMode.Depth;

        var attachDepthTexture = FindObjectOfType<DepthTextureController>();
        Debug.Assert(attachDepthTexture != null,
                     "DepthTextureController must be present in the scene.");
        attachDepthTexture.Materials.Add(m_OcclusionMaterial);

        m_CommandBuffer = new CommandBuffer();
        m_CommandBuffer.name = "Auxilary occlusion textures";

        // Creates the occlusion map.
        int occlusionMapTextureID = Shader.PropertyToID("_OcclusionMap");
        m_CommandBuffer.GetTemporaryRT(occlusionMapTextureID, -1, -1, 0, FilterMode.Bilinear);

        // Pass #0 renders an auxilary buffer - occlusion map that indicates the
        // regions of virtual objects that are behind real geometry.
        m_CommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, occlusionMapTextureID,
          m_OcclusionMaterial, /*pass=*/ 0);

        // Blurs the occlusion map.
        if (m_BlurMaterial == null)
        {
            m_CommandBuffer.SetGlobalTexture("_OcclusionMapBlurred", occlusionMapTextureID);
        }
        else
        {
            int tempRenderID = Shader.PropertyToID("_TempTexture");
            m_CommandBuffer.GetTemporaryRT(tempRenderID, -1, -1, 0, FilterMode.Bilinear);

            {
                int blurRenderID = Shader.PropertyToID("_OcclusionMapBlurred");
                m_CommandBuffer.GetTemporaryRT(blurRenderID, -1 * BlurDownsample,
                  -1 * BlurDownsample, 0, FilterMode.Bilinear);
                m_CommandBuffer.Blit(occlusionMapTextureID, tempRenderID, m_BlurMaterial,
                  /*pass=*/ 0);
                m_CommandBuffer.Blit(tempRenderID, blurRenderID, m_BlurMaterial, /*pass=*/ 1);
                m_CommandBuffer.SetGlobalTexture("_OcclusionMapBlurred", blurRenderID);
            }
        }

        m_Camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, m_CommandBuffer);
        m_Camera.AddCommandBuffer(CameraEvent.AfterGBuffer, m_CommandBuffer);
    }

    private void Update()
    {
        float targetOcclusionTransparency = OcclusionEnabled ? OcclusionTransparency : 0.0f;
        m_CurrentOcclusionTransparency +=
          (targetOcclusionTransparency - m_CurrentOcclusionTransparency) *
          Time.deltaTime * OcclusionFadeVelocity;

        m_CurrentOcclusionTransparency =
          Mathf.Clamp(m_CurrentOcclusionTransparency, 0.0f, OcclusionTransparency);
        m_OcclusionMaterial.SetFloat("_OcclusionTransparency", m_CurrentOcclusionTransparency);
        m_OcclusionMaterial.SetFloat("_TransitionSizeMeters", TransitionSizeMeters);
        Shader.SetGlobalFloat("_BlurSize", BlurSize / BlurDownsample);
    }

    private void OnEnable()
    {
        LifecycleManager.Instance.OnSessionSetEnabled += OnSessionSetEnabled;

        if (m_CommandBuffer != null)
        {
            m_Camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, m_CommandBuffer);
            m_Camera.AddCommandBuffer(CameraEvent.AfterGBuffer, m_CommandBuffer);
        }
    }

    private void OnDisable()
    {
        LifecycleManager.Instance.OnSessionSetEnabled -= OnSessionSetEnabled;

        if (m_CommandBuffer != null)
        {
            m_Camera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, m_CommandBuffer);
            m_Camera.RemoveCommandBuffer(CameraEvent.AfterGBuffer, m_CommandBuffer);
        }
    }

    private void OnSessionSetEnabled(bool sessionEnabled)
    {
        m_SessionEnabled = sessionEnabled;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!m_SessionEnabled)
        {
            return;
        }

        // Pass #1 combines virtual and real cameras based on the occlusion map.
        Graphics.Blit(source, destination, m_OcclusionMaterial, /*pass=*/ 1);
    }
}
