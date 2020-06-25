//-----------------------------------------------------------------------
// <copyright file="ShadowReceiverMesh.cs" company="Google">
//
// Copyright 2019 Google LLC. All Rights Reserved.
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

namespace GoogleARCore
{
    using System.Collections;
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;

    /// <summary>
    /// The ShadowReceiverMesh is a sample to show how one could use the depth
    /// texture to generate a mesh to represent the current scene and accept
    /// shadows. This is a sample.
    /// </summary>
    public class ShadowReceiverMesh : MonoBehaviour
    {
#if UNITY_2017_1_OR_NEWER

        // A small default texture size to create a texture of unknown size.
        private const int k_DefaultTextureSize = 2;

        private static readonly Vector3 k_DefaultMeshOffset = new Vector3(-100, -100, -100);

        // Holds the vertex and index data of the depth template mesh.
        private Mesh m_Mesh;

        // Holds the calibrated camera's intrinsic parameters.
        private CameraIntrinsics m_Intrinsics;

        // This is the scale vector to appropriately scale the camera
        // intrinsics to the depth texture.
        private Vector2 m_IntrinsicsScale;

        private bool m_Initialized = false;

        // Updates every frame with the latest depth data.
        private Texture2D m_DepthTexture;

        private Matrix4x4 m_ScreenRotation = Matrix4x4.Rotate(Quaternion.identity);

        private static int[] GenerateTriangles(int width, int height)
        {
            int[] indices = new int[(height - 1) * (width - 1) * 6];
            int idx = 0;
            for (int y = 0; y < (height - 1); y++)
            {
                for (int x = 0; x < (width - 1); x++)
                {
                    // Unity has a clockwise triangle winding order.
                    // Upper quad triangle
                    int idx0 = (y * width) + x; // Top left
                    int idx1 = idx0 + 1;        // Top right
                    int idx2 = idx0 + width;    // Bottom left

                    // Lower quad triangle
                    int idx3 = idx1;            // Top right
                    int idx4 = idx2 + 1;        // Bottom right
                    int idx5 = idx2;            // Bottom left

                    indices[idx++] = idx0;
                    indices[idx++] = idx1;
                    indices[idx++] = idx2;
                    indices[idx++] = idx3;
                    indices[idx++] = idx4;
                    indices[idx++] = idx5;
                }
            }

            return indices;
        }

        private void InitializeMesh()
        {
            // Get the camera parameters to create the required number of vertices.
            m_Intrinsics = Frame.CameraImage.TextureIntrinsics;

            // Scale camera intrinsics to the depth map size.
            m_IntrinsicsScale.x = m_DepthTexture.width / (float)m_Intrinsics.ImageDimensions.x;
            m_IntrinsicsScale.y = m_DepthTexture.height / (float)m_Intrinsics.ImageDimensions.y;

            // Create template vertices.
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();

            // Create template vertices for the mesh object.
            for (int y = 0; y < m_DepthTexture.height; y++)
            {
                for (int x = 0; x < m_DepthTexture.width; x++)
                {
                    Vector3 v = new Vector3(x * 0.01f, -y * 0.01f, 0) + k_DefaultMeshOffset;
                    vertices.Add(v);
                    normals.Add(Vector3.back);
                }
            }

            // Create template triangle list.
            int[] triangles = GenerateTriangles(m_DepthTexture.width, m_DepthTexture.height);

            // Create the mesh object and set all template data.
            m_Mesh = new Mesh();
            m_Mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            m_Mesh.SetVertices(vertices);
            m_Mesh.SetNormals(normals);
            m_Mesh.SetTriangles(triangles, 0);
            m_Mesh.bounds = new Bounds(Vector3.zero, new Vector3(1000, 1000, 1000));
            m_Mesh.UploadMeshData(true);

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = m_Mesh;

            // Set camera intrinsics for depth reprojection.
            Material material = GetComponent<Renderer>().material;
            material.SetFloat("_FocalLengthX",
                m_Intrinsics.FocalLength.x * m_IntrinsicsScale.x);
            material.SetFloat("_FocalLengthY",
                m_Intrinsics.FocalLength.y * m_IntrinsicsScale.y);
            material.SetFloat("_PrincipalPointX",
                m_Intrinsics.PrincipalPoint.x * m_IntrinsicsScale.x);
            material.SetFloat("_PrincipalPointY",
                m_Intrinsics.PrincipalPoint.y * m_IntrinsicsScale.y);
            material.SetInt("_ImageDimensionsX", m_DepthTexture.width);
            material.SetInt("_ImageDimensionsY", m_DepthTexture.height);

            m_Initialized = true;
        }

        private void Start()
        {
            // Removes any legacy manual mesh rotation to work for portrait mode phone.
            transform.localRotation = Quaternion.identity;

            // Default texture, will be updated each frame.
            m_DepthTexture = new Texture2D(k_DefaultTextureSize, k_DefaultTextureSize);

            // Assign the texture to the material.
            Material material = GetComponent<Renderer>().material;
            material.SetTexture("_CurrentDepthTexture", m_DepthTexture);
            UpdateScreenOrientation();
        }

        private void Update()
        {
            // Get the latest depth data from ARCore.
            Frame.CameraImage.UpdateDepthTexture(ref m_DepthTexture);
            UpdateScreenOrientation();

            if (!m_Initialized
                && m_DepthTexture.width != k_DefaultTextureSize
                && m_DepthTexture.height != k_DefaultTextureSize)
            {
                InitializeMesh();
            }
        }

        private void UpdateScreenOrientation()
        {
            switch (Screen.orientation)
            {
                case ScreenOrientation.Portrait:
                    m_ScreenRotation = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
                    break;
                case ScreenOrientation.LandscapeLeft:
                    m_ScreenRotation = Matrix4x4.Rotate(Quaternion.identity);
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    m_ScreenRotation = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -90));
                    break;
                case ScreenOrientation.LandscapeRight:
                    m_ScreenRotation = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180));
                    break;
            }

            Material material = GetComponent<Renderer>().material;
            material.SetMatrix("_ScreenRotation", m_ScreenRotation);
        }
#endif
    }
}
