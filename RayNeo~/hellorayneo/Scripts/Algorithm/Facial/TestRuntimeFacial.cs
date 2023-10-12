using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RayNeo.Native;
using FFalcon.XR.Runtime;
using RayNeo;

public class TestRuntimeFacial : MonoBehaviour
{
    #region 数据定义
    //面部外框
    public Text Distance;
    #endregion

    #region 生命周期
    private void Update()
    {
        Vector3 pos = FaceDetectorManager.Ins.GetFacePosition(out bool suc);
        if (!suc)
        {
            //获取脸部信息失败.
            return;
        }
        //当前假的.加入偏移值
        //FaceFrame.position = Vector3.Lerp(FaceFrame.position, new Vector3(pos.x - 0.1f, pos.y - 0.1f, Math.Abs(pos.z) + 0.3f), Time.deltaTime * 20);
        //这是正式的代码.先保留.
        
        //transform.localPosition = Vector3.Lerp(transform.position, new Vector3(pos.x, pos.y, pos.z + 0.2f), Time.deltaTime * 20);
        transform.position = Vector3.Lerp(transform.position, Camera.main.transform.TransformPoint(new Vector3(pos.x, pos.y, pos.z + 0.2f)), Time.deltaTime * 15);

        Distance.text = transform.position.z.ToString("f02") + "米";


        transform.LookAt(Camera.main.transform);

    }
    private void OnDestroy()
    {
        FaceDetectorManager.Ins.StopFaceDectector();
    }
    #endregion
}
