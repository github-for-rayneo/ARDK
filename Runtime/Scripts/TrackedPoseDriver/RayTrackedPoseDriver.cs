using FfalconXR.InputModule;
using FfalconXR.Native;
using UnityEngine;

namespace RayNeo.Native
{
    public class RayTrackedPoseDriver : MonoBehaviour
    {
        private void Start()
        {
            SetupInputKeyHandler();
        }

        //private void Update()
        //{
        //    Quaternion quaternion = NativeInfo.Instance.GetMobileQualternion();

        //    if (quaternion != Quaternion.identity)
        //    {
        //        transform.rotation = quaternion;
        //    }
        //}

        private void SetupInputKeyHandler()
        {
#if ENABLE_INPUT_SYSTEM
            if (!GameObject.Find("NewInputSystemKeyHandler"))
            {
                NewInputSystemKeyHandler keyHandler = new GameObject("NewInputSystemKeyHandler").AddComponent<NewInputSystemKeyHandler>();
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (!GameObject.Find("UnityInputKeyHandler"))
            {
                UnityInputKeyHandler keyHandler = new GameObject("UnityInputKeyHandler").AddComponent<UnityInputKeyHandler>();
            }
#endif
        }
    }
}
