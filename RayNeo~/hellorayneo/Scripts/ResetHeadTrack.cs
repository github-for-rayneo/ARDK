using FfalconXR;
using RayNeo.Native;
using RayNeo.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetHeadTrack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TouchEventCtrl.Instance.OnDoubleTap += OnReset;
    }

    private void OnDestroy()
    {
        TouchEventCtrl.Instance.OnDoubleTap -= OnReset;

    }
    private void OnReset()
    {
        HeadTrackedPoseDriver.ResetQuaternion();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
