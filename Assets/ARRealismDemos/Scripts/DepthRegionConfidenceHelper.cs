//-----------------------------------------------------------------------
// <copyright file="DepthRegionConfidenceHelper.cs" company="Google">
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
    using GoogleARCore;
    using UnityEngine;

#if UNITY_IOS && !UNITY_EDITOR
    using AndroidImport = GoogleARCoreInternal.DllImportNoop;
    using IOSImport = System.Runtime.InteropServices.DllImportAttribute;
#else
    using AndroidImport = System.Runtime.InteropServices.DllImportAttribute;
    using IOSImport = GoogleARCoreInternal.DllImportNoop;
#endif

    internal class DepthRegionConfidenceHelper
    {
        public static float getDepthRegionConfidence(
            int rect_x, int rect_y, int rect_width, int rect_height)
        {
            // The native session provides the session and frame handles.
            var nativeSession = LifecycleManager.Instance.NativeSession;
            if (nativeSession == null)
            {
                Debug.LogError("NativeSession is null.");
                return 0;
            }

            float regionConfidence = 0;
            ExternApi.ArFrame_getDepthRegionConfidence(
                nativeSession.SessionHandle, nativeSession.FrameHandle, rect_x,
                rect_y, rect_width, rect_height, ref regionConfidence);

            return regionConfidence;
        }

        private struct ExternApi
        {
#pragma warning disable 626
            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArFrame_getDepthRegionConfidence(
                IntPtr session, IntPtr frame, int rect_x, int rect_y,
                int rect_width, int rect_height, ref float out_region_confidence);
#pragma warning restore 626
        }
    }
}
