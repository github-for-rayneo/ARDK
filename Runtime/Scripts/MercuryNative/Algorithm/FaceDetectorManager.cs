using FFalcon.XR.Runtime;
using UnityEngine;
namespace RayNeo
{

    public class FaceDetectorManager
    {
        private static FaceDetectorManager ins = new FaceDetectorManager();
        public static FaceDetectorManager Ins
        {
            get
            {
                return ins;
            }
        }

        float[] m_position = new float[3];
        Vector3 m_posVec3 = Vector3.zero;
        private long m_faceHandle = 0;

        /// <summary>
        /// ��ȡ����λ��. 
        /// ���ü������ʼ��.��Ҫ���ʵ�ʱ������StopFaceDectector
        /// </summary>
        /// <param name="suc">������û�л�ȡ������.</param>
        /// <returns>�����Vector3.zero����û�л�ȡ��.</returns>

        public Vector3 GetFacePosition(out bool suc)
        {
#if UNITY_EDITOR
            //�༭����ִ��. �������Կ��Ǽ���debug
            suc = false;
            return Vector3.zero;
#endif
            if (m_faceHandle == 0)
            {
                m_faceHandle = Api.CreateFaceDetector();
                if (m_faceHandle != 0)
                {
                    Api.InitFaceDetector();
                }
            }
            if (m_faceHandle != 0)
            {
                Api.GetFaceInCamera(m_position);
                //Api.GetFacePosition(m_position);
                if (m_position[0] == 0 && m_position[1] == 0 && m_position[2] == 0)
                {
                    suc = false;
                    return Vector3.zero;
                }
                m_posVec3.Set(m_position[0], m_position[1], m_position[2]);
            }
            else
            {
                suc = false;
                return Vector3.zero;
            }
            suc = true;
            return m_posVec3;
        }

        public void StopFaceDectector()
        {
#if UNITY_EDITOR
            return;
#endif
            Api.DestroyFaceDetector();
            m_faceHandle = 0;
        }


        public void Recenter()
        {
#if UNITY_EDITOR
            return;
#endif
            Api.Recenter();
        }
    }
}
