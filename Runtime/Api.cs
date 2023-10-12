namespace FFalcon.XR.Runtime
{
    using AOT;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class Api
    {
        private static bool mIsDeviceConfigChanged;
        private static ScreenOrientation mLastScreenOrientation;
        public delegate void FXRStateEventCallback(UInt32 state, UInt64 timestamp, uint length, IntPtr data);
        private static List<FXRStateEventCallback> mStateEventCallbackLists = new List<FXRStateEventCallback>();

        public static void ReloadDeviceParams()
        {
            if (!XRLoader.mIsInitialized)
            {
                return;
            }
            Debug.Log("[FFalconXR] Reload device parameters.");
            FXRUnity_SetDeviceParamsChanged();
            mIsDeviceConfigChanged = false;
        }

        public static void UpdateScreenParams()
        {
            if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before UpdateScreenParams.");
                return;
            }

            if (mLastScreenOrientation != Screen.orientation)
            {
                XRLoader.RecalculateRectangles(Screen.safeArea);
                mLastScreenOrientation = Screen.orientation;
                mIsDeviceConfigChanged = true;
            }

            if (mIsDeviceConfigChanged)
            {
                ReloadDeviceParams();
            }
        }

        public static void Recenter()
        {
            if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before Recenter.");
                return;
            }
            FXRUnity_RecenterHeadTracker();
        }

        public static void EnableSlamHeadTracker()
        {
            if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before EnableSlamHeadTracker.");
                return;
            }
            FXRUnity_EnableSlamHeadTracker();
        }

        public static void DisableSlamHeadTracker()
        {
            if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before DisableSlamHeadTracker.");
                return;
            }
            FXRUnity_DisableSlamHeadTracker();
        }

        public static float[] GetNineAxisOrientation(float[] orientation)
        {
            if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before GetNineAxisOrientation.");
                return orientation;
            }
            FXRUnity_NineAxisOrientation(orientation);
            return orientation;
        }
		
		public static int GetHeadTrackerStatus(){
			if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before GetHeadTrackerStatus.");
                return -1;
            }
            return FXRUnity_GetHeadTrackerStatus();
		}

        [MonoPInvokeCallback(typeof(FXRStateEventCallback))]
        private static void StateEventDispatcher(UInt32 state, UInt64 timestamp, uint length, IntPtr data)
        {
            foreach (FXRStateEventCallback item in mStateEventCallbackLists)
            {
                item(state, timestamp, length, data);
            }
        }

        public static bool RegisterStateEventCallback(FXRStateEventCallback callback)
        {
            if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before RegisterStateEventCallback.");
                return false;
            }
            if (mStateEventCallbackLists.Contains(callback)) return false;
            mStateEventCallbackLists.Add(callback);
            if (mStateEventCallbackLists.Count == 1)
            {
                FXRUnity_RegisterStateEventCallback(Marshal.GetFunctionPointerForDelegate<FXRStateEventCallback>(StateEventDispatcher));
            }
            return true;
        }

        public static bool UnregisterStateEventCallback(FXRStateEventCallback callback)
        {
            if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before UnregisterStateEventCallback.");
                return false;
            }
            if (!mStateEventCallbackLists.Contains(callback)) return false;
            mStateEventCallbackLists.Remove(callback);
            if(mStateEventCallbackLists.Count == 0)
            {
                FXRUnity_UnregisterStateEventCallback(Marshal.GetFunctionPointerForDelegate<FXRStateEventCallback>(StateEventDispatcher));
            }
            return true;
        }

        public static int SendCommand(int unit, int command)
        {
            if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before SendCommand.");
                return -1;
            }
            return FXRUnity_SendCommand(unit, command);
        }

        public static void EnablePlaneDetection()
        {
            if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before EnablePlaneDetection.");
            }
            FXRUnity_EnablePlaneDetection();
        }

        public static void DisablePlaneDetection()
        {
            if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before EnablePlaneDetection.");
            }
            FXRUnity_DisablePlaneDetection();
        }

        public static int GetPlaneInfo(XRPlaneInfo[] info, int arraySize)
        {
            if (!XRLoader.mIsInitialized)
            {
                Debug.LogError("Please initialize FFalcon XR loader before GetPlaneInfo.");
                return 0;
            }
            return FXRUnity_GetPlaneInfo(info, arraySize);
        }

        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_SetDeviceParamsChanged();

        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_RecenterHeadTracker();

        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_EnableSlamHeadTracker();

        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_DisableSlamHeadTracker();
		
        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_NineAxisOrientation(float[] orientation);
        [DllImport(ApiConstants.FxrApi)]
        public static extern float FXRUnity_NineAxisAzimuth();

        [DllImport(ApiConstants.FxrApi)]
        private static extern int FXRUnity_GetHeadTrackerStatus();
		
		[DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_RegisterStateEventCallback(IntPtr callback);

        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_UnregisterStateEventCallback(IntPtr callback);

        [DllImport(ApiConstants.FxrApi)]
        private static extern int FXRUnity_SendCommand(int unit, int command);

        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_EnablePlaneDetection();

        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_DisablePlaneDetection();

        [DllImport(ApiConstants.FxrApi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FXRUnity_GetPlaneInfo([In, Out] XRPlaneInfo[] info, int arraySize);

        public static long CreateFaceDetector()
        {
            IntPtr pHandle;
            if (FXRFeature_CreateFaceDetector(out pHandle) == 0)
            {
                return pHandle.ToInt64();
            }
            return 0;
        }

        public static void InitFaceDetector()
        {
            FXRFeature_InitFaceDetector();
        }

        public static void DestroyFaceDetector()
        {
            FXRFeature_DestroyFaceDetector();
        }

        public static void SetCameraFrameRate(int fps) {
            FXRFeature_SetFrameRate(fps);
        }

        public static float[] GetFacePosition(float[] position)
        {
            FXRFeature_GetFacePosition(position, position.Length);
            return position;
        }

        public static float[] GetFaceInCamera(float[] position)
        {
            FXRFeature_GetFaceInCamera(position, position.Length);
            return position;
        }

        public static float[] GetFaceLookAt()
        {
            float[] lookAt = new float[3];
            FXRFeature_GetFaceLookAt(lookAt, lookAt.Length);
            return lookAt;
        }

        public static bool CheckFaceState()
        {
            return FXRFeature_CheckFaceState();
        }

        [DllImport(ApiConstants.FxrAlgorithm)]
        private static extern int FXRFeature_CreateFaceDetector(out IntPtr handle);

        [DllImport(ApiConstants.FxrAlgorithm)]
        private static extern void FXRFeature_InitFaceDetector();

        [DllImport(ApiConstants.FxrAlgorithm)]
        private static extern void FXRFeature_DestroyFaceDetector();

        [DllImport(ApiConstants.FxrAlgorithm)]
        private static extern void FXRFeature_GetFacePosition(float[] position, int size);

        [DllImport(ApiConstants.FxrAlgorithm)]
        private static extern void FXRFeature_GetFaceInCamera(float[] position, int size);

        [DllImport(ApiConstants.FxrAlgorithm)]
        private static extern void FXRFeature_GetFaceLookAt(float[] lookAt, int size);

        [DllImport(ApiConstants.FxrAlgorithm)]
        private static extern void FXRFeature_SetFrameRate(int fps);

        [DllImport(ApiConstants.FxrAlgorithm)]
        private static extern bool FXRFeature_CheckFaceState();


    }
}