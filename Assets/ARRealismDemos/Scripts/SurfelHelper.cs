//-----------------------------------------------------------------------
// <copyright file="SurfelHelper.cs" company="Google">
//
// Copyright 2020 Google Inc. All Rights Reserved.
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

namespace GoogleARCoreInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using GoogleARCore;
    using GoogleARCoreInternal;
    using UnityEngine;

#if UNITY_IOS && !UNITY_EDITOR
    using AndroidImport = GoogleARCoreInternal.DllImportNoop;
    using IOSImport = System.Runtime.InteropServices.DllImportAttribute;
#else
    using AndroidImport = System.Runtime.InteropServices.DllImportAttribute;
    using IOSImport = GoogleARCoreInternal.DllImportNoop;
#endif

    /// <summary>
    /// Experimental container to support surfel data retrieval.
    /// </summary>
    internal class SurfelHelper
    {
        private static float[] s_PositionsArray = new float[0];
        private static float[] s_NormalsArray = new float[0];
        private static float[] s_RadiiArray = new float[0];
        private static byte[] s_ColorsArray = new byte[0];
        private static int s_NumSurfels = 0;

        /// <summary>
        /// Gets the array of positions of the latest surfel data.
        /// </summary>
        public static float[] Positions
        {
            get
            {
                return s_PositionsArray;
            }
        }

        /// <summary>
        /// Gets the array of normals of the latest surfel data.
        /// </summary>
        public static float[] Normals
        {
            get
            {
                return s_NormalsArray;
            }
        }

        /// <summary>
        /// Gets the array of radii of the latest surfel data.
        /// </summary>
        public static float[] Radii
        {
            get
            {
                return s_RadiiArray;
            }
        }

        /// <summary>
        /// Gets the array of colors of the latest surfel data.
        /// </summary>
        public static byte[] Colors
        {
            get
            {
                return s_ColorsArray;
            }
        }

        /// <summary>
        /// Gets the current number of surfels.
        /// </summary>
        public static int NumSurfels
        {
            get
            {
                return s_NumSurfels;
            }
        }

        /// <summary>
        /// Updates surfel data.
        /// </summary>
        /// <returns>
        /// True, if successfully updated surfels.
        /// </returns>
        public static bool UpdateSurfels()
        {
            // The native session provides the session and frame handles.
            var nativeSession = LifecycleManager.Instance.NativeSession;
            if (nativeSession == null)
            {
                Debug.LogError("NativeSession is null.");
                return false;
            }

            IntPtr surfelListHandle = IntPtr.Zero;
            ApiArStatus status = (ApiArStatus)ExternApi.ArSurfelList_acquire(
              nativeSession.SessionHandle, ref surfelListHandle);
            if (status != ApiArStatus.Success)
            {
                ExternApi.ArSurfelList_release(surfelListHandle);
                Debug.LogError("SurfelHelper, status: " + status);
                return false;
            }

            // Get the number of surfels.
            ExternApi.ArSurfelList_getSize(nativeSession.SessionHandle, surfelListHandle,
                                           ref s_NumSurfels);

            if (s_NumSurfels > 0)
            {
                Array.Resize(ref s_PositionsArray, s_NumSurfels * 3);
                Array.Resize(ref s_NormalsArray, s_NumSurfels * 3);
                Array.Resize(ref s_RadiiArray, s_NumSurfels);
                Array.Resize(ref s_ColorsArray, s_NumSurfels);

                // Get positions.
                {
                    IntPtr positionsPtr = IntPtr.Zero;
                    ExternApi.ArSurfelList_getSurfelPositions(nativeSession.SessionHandle,
                                                              surfelListHandle, ref positionsPtr);
                    IntPtr positionsDataPtr = new IntPtr(positionsPtr.ToInt64());
                    Marshal.Copy(positionsDataPtr, s_PositionsArray as float[], 0,
                                 s_PositionsArray.Length);
                }

                // Get normals.
                {
                    IntPtr normalsPtr = IntPtr.Zero;
                    ExternApi.ArSurfelList_getSurfelNormals(nativeSession.SessionHandle,
                                                            surfelListHandle, ref normalsPtr);
                    IntPtr normalsDataPtr = new IntPtr(normalsPtr.ToInt64());
                    Marshal.Copy(normalsDataPtr, s_NormalsArray as float[], 0,
                                 s_NormalsArray.Length);
                }

                // Get radii.
                {
                    IntPtr radiiPtr = IntPtr.Zero;
                    ExternApi.ArSurfelList_getSurfelRadii(nativeSession.SessionHandle,
                                                          surfelListHandle, ref radiiPtr);
                    IntPtr radiiDataPtr = new IntPtr(radiiPtr.ToInt64());
                    Marshal.Copy(radiiDataPtr, s_RadiiArray as float[], 0, s_RadiiArray.Length);
                }

                // Get colors.
                {
                    IntPtr colorsPtr = IntPtr.Zero;
                    ExternApi.ArSurfelList_getSurfelColors(nativeSession.SessionHandle,
                                                           surfelListHandle, ref colorsPtr);
                    IntPtr colorsDataPtr = new IntPtr(colorsPtr.ToInt64());
                    Marshal.Copy(colorsDataPtr, s_ColorsArray as byte[], 0, s_ColorsArray.Length);
                }
            }

            ExternApi.ArSurfelList_release(surfelListHandle);

            return true;
        }

        private struct ExternApi
        {
#pragma warning disable 626
            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern ApiArStatus ArSurfelList_acquire(IntPtr sessionHandle,
                                                                  ref IntPtr surfelListHandle);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArSurfelList_release(IntPtr surfelListHandle);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArSurfelList_getSize(IntPtr sessionHandle,
                                                           IntPtr surfelListHandle,
                                                           ref int numSurfels);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArSurfelList_getSurfelPositions(IntPtr sessionHandle,
                                                IntPtr surfelListHandle,
                                                ref IntPtr outSurfelPositionsXyz);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArSurfelList_getSurfelNormals(IntPtr sessionHandle,
                                            IntPtr surfelListHandle,
                                            ref IntPtr outSurfelNormalsXyz);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArSurfelList_getSurfelRadii(IntPtr sessionHandle,
                                            IntPtr surfelListHandle,
                                            ref IntPtr outSurfelRadii);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArSurfelList_getSurfelColors(IntPtr sessionHandle,
                                            IntPtr surfelListHandle,
                                            ref IntPtr outSurfelColors);
#pragma warning restore 626
        }
    }
}
