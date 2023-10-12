using UnityEngine;
using FfalconXR;

namespace RayNeo.Native
{
    public class XRSDK
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Load()
        {
            Initialize();
        }

        private static void Initialize()
        {
            Log.Init();
            Application.targetFrameRate = XRSDKConfig.Instance.TargetFrameRate;
            Screen.sleepTimeout = XRSDKConfig.Instance.SleepTimeOut;
        }
    }
}
