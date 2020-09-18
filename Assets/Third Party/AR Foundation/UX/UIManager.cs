using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The ARCameraManager which will produce frame events.")]
        ARCameraManager m_CameraManager;

        /// <summary>
        /// Get or set the <c>ARCameraManager</c>.
        /// </summary>
        public ARCameraManager cameraManager
        {
            get { return m_CameraManager; }
            set
            {
                if (m_CameraManager == value)
                    return;

                if (m_CameraManager != null)
                    m_CameraManager.frameReceived -= FrameChanged;

                m_CameraManager = value;

                if (m_CameraManager != null & enabled)
                    m_CameraManager.frameReceived += FrameChanged;
            }
        }

        const string k_FadeOffAnim = "FadeOff";
        const string k_FadeOnAnim = "FadeOn";

        [SerializeField]
        ARPlaneManager m_PlaneManager;

        public ARPlaneManager planeManager
        {
            get { return m_PlaneManager; }
            set { m_PlaneManager = value; }
        }

        [SerializeField]
        Animator m_MoveDeviceAnimation;

        public Animator moveDeviceAnimation
        {
            get { return m_MoveDeviceAnimation; }
            set { m_MoveDeviceAnimation = value; }
        }

        Animator m_TapToPlaceAnimation;

        public Animator tapToPlaceAnimation
        {
            get { return m_TapToPlaceAnimation; }
            set { m_TapToPlaceAnimation = value; }
        }

        public GameObject handAnimationARCore;

        static List<ARPlane> s_Planes = new List<ARPlane>();

        bool m_ShowingTapToPlace = false;

        bool m_ShowingMoveDevice = true;

        public GameObject mainUI;

        void OnEnable()
        {
            if (m_CameraManager != null)
                m_CameraManager.frameReceived += FrameChanged;

            DominoPlacing.onPlacedObject += PlacedObject;
            handAnimationARCore.SetActive(true);
        }

        void OnDisable()
        {
            if (m_CameraManager != null)
                m_CameraManager.frameReceived -= FrameChanged;

            DominoPlacing.onPlacedObject -= PlacedObject;
        }

        void FrameChanged(ARCameraFrameEventArgs args)
        {
            if (PlanesFound() && m_ShowingMoveDevice)
            {
                if (handAnimationARCore)
                {
                    handAnimationARCore.SetActive(false);
                }

                if (moveDeviceAnimation)
                    moveDeviceAnimation.SetTrigger(k_FadeOffAnim);

                //if (tapToPlaceAnimation)
                //    tapToPlaceAnimation.SetTrigger(k_FadeOnAnim);

                m_ShowingTapToPlace = true;
                m_ShowingMoveDevice = false;
                mainUI.SetActive(true);
                mainUI.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
                StartCoroutine(CanvasToggle());
            }
        }
        IEnumerator CanvasToggle()
        {
            yield return new WaitForSeconds(0.5f);
            mainUI.transform.GetChild(0).GetComponent<Canvas>().enabled = true;
        }

        bool PlanesFound()
        {
            if (planeManager == null)
                return false;

            return planeManager.trackables.count > 0;
        }

        void PlacedObject()
        {
            if (m_ShowingTapToPlace)
            {
                if (tapToPlaceAnimation)
                    tapToPlaceAnimation.SetTrigger(k_FadeOffAnim);

                m_ShowingTapToPlace = false;
            }
        }
    }
}