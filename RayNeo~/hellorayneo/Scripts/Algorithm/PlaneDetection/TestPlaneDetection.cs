using FFalcon.XR.Runtime;
using RayNeo;
using RayNeo.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class TestPlaneDetection : MonoBehaviour
{
    XRPlaneInfo[] m_infoArrays = new XRPlaneInfo[3];

    private List<GameObject> m_planeObjs = new List<GameObject>();

    public Text m_tips;


    private void Awake()
    {
#if UNITY_EDITOR
        m_tips.text = "请在眼镜端执行.";
#else
        AlgorithmApi.EnableSlamHeadTracker();
        AlgorithmApi.EnablePlaneDetection();
#endif
        for (int i = 0; i < m_infoArrays.Length; i++)
        {
            var go = new GameObject("Plane" + i);
            m_planeObjs.Add(go);
        }
    }



    private void Update()
    {

#if UNITY_EDITOR
        //以下是编辑器中的测试代码
        int res = 1;
        m_infoArrays[0].local_polygon = new float[] { 0.6148487f, -0.3152966f, -0.3228481f, -0.5776324f, -0.3994534f, -0.4614441f, -0.4951928f, -0.2272373f, -0.6158717f, 0.1089409f, -0.1364577f, 0.5693948f, -0.0353812f, 0.5776324f, 0.3512281f, 0.5653926f, 0.5454746f, 0.3796201f, 0.591276f, 0.07411952f, 0.6158718f, -0.2284977f };
        m_infoArrays[0].local_polygon_size = 11;
        m_infoArrays[0].pose.position.x = 1.03f;
        m_infoArrays[0].pose.position.y = -1.16f;
        m_infoArrays[0].pose.position.z = 0.44f;
        var q = new Quaternion(-0.10573f, 0.69916f, 0.69916f, 0.10573f);
        m_infoArrays[0].pose.rotation.x = q.x;
        m_infoArrays[0].pose.rotation.y = q.y;
        m_infoArrays[0].pose.rotation.z = q.z;
        m_infoArrays[0].pose.rotation.w = q.w;

#else
        int res = AlgorithmApi.GetPlaneInfo(m_infoArrays);
 
#endif
        Debug.Log("TestPlaneDetection获取平面信息---z序列:" + res + ":" + m_infoArrays.Length);
        for (int i = 0; i < m_infoArrays.Length; i++)
        {
            if (i < res)
            {
                m_planeObjs[i].SetActive(true);
                XRPlaneInfo info = m_infoArrays[i];
                Debug.Log("TestPlaneDetection 开始创建模型:" + info.local_polygon.Length + ":" + info.local_polygon_size);

                GameObject obj = AlgorithmApi.CreatePlaneMesh(info, m_planeObjs[i], true, true);
                obj.transform.localPosition = AlgorithmApi.ConvertPlanePosition(info);
                obj.transform.localRotation = AlgorithmApi.ConvertPlaneRotation(info);
                var mesh = obj.GetComponent<MeshFilter>().mesh;
                for (int j = 0; j < mesh.vertices.Length; j++)
                {
                    var wp = obj.transform.TransformPoint(mesh.vertices[j]);
                }
                Debug.Log("TestPlaneDetection 模型创建完毕:" + obj.transform.localPosition + ":" + obj.transform.localRotation + ":" + obj.transform.localRotation.eulerAngles);

            }
            else
            {
                m_planeObjs[i].SetActive(false);
            }
        }

    }

    public void TestCameraMaterix()
    {

    }

    private void OnDestroy()
    {
#if UNITY_EDITOR

#else
        AlgorithmApi.DisablePlaneDetection();
        AlgorithmApi.DisableSlamHeadTracker();

#endif
        RecordManager.Ins.StopRecord();

    }
}
