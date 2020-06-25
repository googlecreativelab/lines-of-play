//-----------------------------------------------------------------------
// <copyright file="MotionStereoController.cs" company="Google">
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

namespace GoogleARCore.Examples.MotionStereo
{
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    /// <summary>
    /// Controls the MotionStereo example.
    /// </summary>
    public class MotionStereoController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR
        /// background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing.
        /// </summary>
        public GameObject PlacementObjectPrefab;

        /// <summary>
        /// UI Object used to select visualization mode.
        /// </summary>
        public Dropdown ModeSelector;

        /// <summary>
        /// Scene object for the Android stuck to the camera.
        /// </summary>
        public GameObject SelfieStickObject;

        /// <summary>
        /// Scene object for the shadow mesh.
        /// </summary>
        public GameObject ShadowMesh;

        /// <summary>
        /// Scene object for visualizing depth data.
        /// </summary>
        public GameObject DebugVisualizer;

        private const float k_ModelRotation = 180.0f;
        private const string k_DefaultMode = "Camera-attached object";
        private const string k_DepthVisualizer = "Depth Visualizer";
        private const string k_SparseDepthVisualizer = "Sparse Depth Visualizer";
        private const string k_SparseNoReprojection = "Sparse w/o Reprojection";

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error,
        /// otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        /// <summary>
        /// The Unity Start() method.
        /// </summary>
        public void Start()
        {
            // Add all internal test modes.
            ModeSelector.options.Add(new Dropdown.OptionData(k_DefaultMode));
            ModeSelector.options.Add(new Dropdown.OptionData(k_DepthVisualizer));
            ModeSelector.options.Add(new Dropdown.OptionData(k_SparseDepthVisualizer));
            ModeSelector.options.Add(new Dropdown.OptionData(k_SparseNoReprojection));

            // Initial mode update.
            ModeSelector.captionText.text = k_DefaultMode;
            OnTestModeChanged();
        }

        /// <summary>
        /// Changes the current test visualization mode.
        /// </summary>
        public void OnTestModeChanged()
        {
            // Reset and hide all scene test objects.
            SelfieStickObject.SetActive(false);
            ShadowMesh.SetActive(false);
            DebugVisualizer.SetActive(false);
            AttachDepthTexture script =
                DebugVisualizer.GetComponent<AttachDepthTexture>();

            // Enable and reset the correct current elements.
            if (ModeSelector.captionText.text == k_DefaultMode)
            {
                SelfieStickObject.SetActive(true);
                ShadowMesh.SetActive(true);
            }
            else if (ModeSelector.captionText.text == k_DepthVisualizer)
            {
                DebugVisualizer.SetActive(true);
                if (script != null)
                {
                    script.UseSparseDepth = false;
                    script.ReprojectIntermediateSparseDepth = false;
                }
            }
            else if (ModeSelector.captionText.text == k_SparseDepthVisualizer)
            {
                DebugVisualizer.SetActive(true);
                if (script != null)
                {
                    script.UseSparseDepth = true;
                    script.ReprojectIntermediateSparseDepth = true;
                }
            }
            else if (ModeSelector.captionText.text == k_SparseNoReprojection)
            {
                DebugVisualizer.SetActive(true);
                if (script != null)
                {
                    script.UseSparseDepth = true;
                    script.ReprojectIntermediateSparseDepth = false;
                }
            }
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            _UpdateApplicationLifecycle();

            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            // Should not handle input if the player is pointing on UI.
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    // Instantiate Andy model at the hit pose.
                    var andyObject =
                      Instantiate(PlacementObjectPrefab, hit.Pose.position, hit.Pose.rotation);

                    // Compensate for the hitPose rotation facing away from the raycast (i.e.
                    // camera).
                    andyObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                    // Create an anchor to allow ARCore to track the hitpoint as understanding of
                    // the physical world evolves.
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                    // Make Andy model a child of the anchor.
                    andyObject.transform.parent = anchor.transform;
                }
            }
        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to
            // appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage(
                    "ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity =
                unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject =
                        toastClass.CallStatic<AndroidJavaObject>(
                            "makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
