namespace FFalcon.XR.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.XR;
    using UnityEngine.XR.Management;

    public class XRLoader : XRLoaderHelper
    {
        private static List<XRDisplaySubsystemDescriptor> mDisplaySubsystemDescriptors =
            new List<XRDisplaySubsystemDescriptor>();

        private static List<XRInputSubsystemDescriptor> mInputSubsystemDescriptors =
            new List<XRInputSubsystemDescriptor>();


        private enum FXRGraphicsApi
        {
            kOpenGLESv3 = 0,
            kVulkan = 1,
            kNone = -1,
        }

        private enum FFalconXRViewportOrientation
        {
            kLandscapeLeft = 0,
            kLandscapeRight = 1,
            kPortrait = 2,
            kPortraitUpsideDown = 3,
        }

        internal static bool mIsInitialized { get; private set; }

        internal static bool mIsStated { get; private set; }

        public override bool Initialize()
        {
            FXRRuntimeInitialize();
            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(mDisplaySubsystemDescriptors, "FXRDisplay");
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(mInputSubsystemDescriptors, "FXRInput");
            mIsInitialized = true;
            Debug.LogError("XRLoader Initialized");
            return true;
        }

        public override bool Start()
        {
            StartSubsystem<XRDisplaySubsystem>();
            StartSubsystem<XRInputSubsystem>();
            FXRUnity_StartXR();
            mIsStated = true;
            Debug.LogError("XRLoader Started");
            return true;
        }

        public override bool Stop()
        {
            FXRUnity_StopXR();
            StopSubsystem<XRDisplaySubsystem>();
            StopSubsystem<XRInputSubsystem>();
            mIsStated = false;
            Debug.LogError("XRLoader Stopped");
            return true;
        }

        public override bool Deinitialize()
        {
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            FXRRuntimeDeinitialize();
            mIsInitialized = false;
            Debug.LogError("XRLoader Deinitialized");
            return true;
        }

        internal static void RecalculateRectangles(Rect renderingArea)
        {
            FXRUnity_SetScreenParams((int)Screen.width, (int)Screen.height, (int)renderingArea.x, (int)renderingArea.y, (int)renderingArea.width, (int)renderingArea.height);
        }

        private static void SetGraphicsApi()
        {
            switch (SystemInfo.graphicsDeviceType)
            {
                case GraphicsDeviceType.OpenGLES3:
                    FXRUnity_SetGraphicsApi(FXRGraphicsApi.kOpenGLESv3);
                    break;
                default:
                    Debug.LogErrorFormat(
                        "The FFalcon XR Plugin cannot be initialized given that the selected " +
                        "Graphics API ({0}) is not supported. Please use OpenGL ES 3.0.", SystemInfo.graphicsDeviceType);
                    break;
            }
        }

        private void FXRRuntimeInitialize()
        {
#if UNITY_ANDROID
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var context = activity.Call<AndroidJavaObject>("getApplicationContext");

            FXRUnity_InitializeAndroid(activity.GetRawObject());
            var algorithm = new AndroidJavaClass("com.ffalcon.xr.extension.FxrAlgorithm");
            algorithm.CallStatic<bool>("InitAndroid", context);
#endif
            SetGraphicsApi();
            RecalculateRectangles(Screen.safeArea);
        }

        private void FXRRuntimeDeinitialize()
        {

        }

#if UNITY_ANDROID
        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_InitializeAndroid(IntPtr context);
#endif

        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_StartXR();

        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_StopXR();

        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_SetGraphicsApi(FXRGraphicsApi api);

        [DllImport(ApiConstants.FxrApi)]
        private static extern void FXRUnity_SetScreenParams(int displayWidth, int displayHeight, int x, int y, int viewportWidth, int viewportHeight);
    }
}