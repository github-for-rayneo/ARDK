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
        /// 获取脸部位置. 
        /// 调用即代表初始化.需要在适当时机调用StopFaceDectector
        /// </summary>
        /// <param name="suc">代表有没有获取到数据.</param>
        /// <returns>如果是Vector3.zero则是没有获取到.</returns>

        public Vector3 GetFacePosition(out bool suc)
        {
#if UNITY_EDITOR
            //编辑器不执行. 后续可以考虑加入debug
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
