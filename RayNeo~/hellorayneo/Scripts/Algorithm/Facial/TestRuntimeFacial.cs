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
    #region ���ݶ���
    //�沿���
    public Text Distance;
    #endregion

    #region ��������
    private void Update()
    {
        Vector3 pos = FaceDetectorManager.Ins.GetFacePosition(out bool suc);
        if (!suc)
        {
            //��ȡ������Ϣʧ��.
            return;
        }
        //��ǰ�ٵ�.����ƫ��ֵ
        //FaceFrame.position = Vector3.Lerp(FaceFrame.position, new Vector3(pos.x - 0.1f, pos.y - 0.1f, Math.Abs(pos.z) + 0.3f), Time.deltaTime * 20);
        //������ʽ�Ĵ���.�ȱ���.
        
        //transform.localPosition = Vector3.Lerp(transform.position, new Vector3(pos.x, pos.y, pos.z + 0.2f), Time.deltaTime * 20);
        transform.position = Vector3.Lerp(transform.position, Camera.main.transform.TransformPoint(new Vector3(pos.x, pos.y, pos.z + 0.2f)), Time.deltaTime * 15);

        Distance.text = transform.position.z.ToString("f02") + "��";


        transform.LookAt(Camera.main.transform);

    }
    private void OnDestroy()
    {
        FaceDetectorManager.Ins.StopFaceDectector();
    }
    #endregion
}
