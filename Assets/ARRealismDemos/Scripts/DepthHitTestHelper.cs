//-----------------------------------------------------------------------
// <copyright file="DepthHitTestHelper.cs" company="Google">
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

    internal class DepthHitTestHelper
    {
        public static bool HitTest(Vector2 screenPosition, out TrackableHit outTrackableHit)
        {
            outTrackableHit = new TrackableHit();

            // The native session provides the session and frame handles.
            var nativeSession = LifecycleManager.Instance.NativeSession;
            if (nativeSession == null)
            {
                Debug.LogError("NativeSession is null.");
                return false;
            }

            IntPtr hitResultListHandle = IntPtr.Zero;
            ExternApi.ArHitResultList_create(
                nativeSession.SessionHandle, ref hitResultListHandle);

            ExternApi.ArFrame_depthHitTest(
                nativeSession.SessionHandle, nativeSession.FrameHandle, screenPosition.x,
                screenPosition.y, hitResultListHandle);

            int hitListSize = 0;
            ExternApi.ArHitResultList_getSize(
                nativeSession.SessionHandle, hitResultListHandle, ref hitListSize);

            if (hitListSize == 0)
            {
                return false;
            }

            // Depth hit test only returns one hit test per pixel referenced.
            const int itemIndex = 0;

            // Query the hit result.
            IntPtr hitResultHandle = IntPtr.Zero;
            ExternApi.ArHitResult_create(
                nativeSession.SessionHandle, ref hitResultHandle);
            ExternApi.ArHitResultList_getItem(
                nativeSession.SessionHandle, hitResultListHandle, itemIndex, hitResultHandle);
            if (hitResultHandle == IntPtr.Zero)
            {
                ExternApi.ArHitResult_destroy(hitResultHandle);
                return false;
            }

            // Query the pose from hit result.
            IntPtr poseHandle = nativeSession.PoseApi.Create();
            ExternApi.ArHitResult_getHitPose(
                nativeSession.SessionHandle, hitResultHandle, poseHandle);
            Pose hitPose = nativeSession.PoseApi.ExtractPoseValue(poseHandle);

            // Query the distance from hit result.
            float hitDistance = 0.0f;
            ExternApi.ArHitResult_getDistance(
                nativeSession.SessionHandle, hitResultHandle, ref hitDistance);

            // Query the trackable from hit result.
            IntPtr trackableHandle = IntPtr.Zero;
            ExternApi.ArHitResult_acquireTrackable(
                nativeSession.SessionHandle, hitResultHandle, ref trackableHandle);
            Trackable trackable = nativeSession.TrackableFactory(trackableHandle);
            nativeSession.TrackableApi.Release(trackableHandle);

            // Calculate trackable hit flags.
            TrackableHitFlags flag = TrackableHitFlags.None;
            if (trackable == null)
            {
                Debug.Log("Could not create trackable from hit result.");
                nativeSession.PoseApi.Destroy(poseHandle);
                return false;
            }
            else if (trackable is FeaturePoint)
            {
                var point = trackable as FeaturePoint;
                flag |= TrackableHitFlags.FeaturePoint;
                if (point.OrientationMode == FeaturePointOrientationMode.SurfaceNormal)
                {
                    flag |= TrackableHitFlags.FeaturePointWithSurfaceNormal;
                }
            }

            outTrackableHit = new TrackableHit(hitPose, hitDistance, flag, trackable);

            nativeSession.PoseApi.Destroy(poseHandle);
            ExternApi.ArHitResultList_destroy(hitResultListHandle);

            return true;
        }

        private struct ExternApi
        {
#pragma warning disable 626
            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArFrame_depthHitTest(
                IntPtr session, IntPtr frame, float pixel_x, float pixel_y, IntPtr hit_result_list);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArHitResultList_create(
                IntPtr session, ref IntPtr out_hit_result_list);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArHitResultList_destroy(
                IntPtr hit_result_list);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArHitResultList_getSize(
                IntPtr session, IntPtr hit_result_list, ref int out_size);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArHitResultList_getItem(
                IntPtr session, IntPtr hit_result_list, int index, IntPtr out_hit_result);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArHitResult_create(IntPtr session, ref IntPtr out_hit_result);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArHitResult_destroy(IntPtr hit_result);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArHitResult_getDistance(IntPtr session, IntPtr hit_result,
                ref float out_distance);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArHitResult_getHitPose(
                IntPtr session, IntPtr hit_result, IntPtr out_pose);

            [AndroidImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArHitResult_acquireTrackable(
                IntPtr session, IntPtr hit_result, ref IntPtr out_trackable);
#pragma warning restore 626
        }
    }
}
