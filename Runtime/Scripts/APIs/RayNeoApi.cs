using FFalcon.XR.Runtime;
using UnityEngine;
namespace RayNeo
{

    /// <summary>
    /// ��ȡ��ǰActivity
    /// </summary>
    public static class AndroidActivity
    {
        private static AndroidJavaObject m_curAct;

        /// <summary>
        /// ��ȡ��ǰ���е�activity
        /// </summary>
        public static AndroidJavaObject CurActivity
        {
            get
            {
                if (m_curAct == null)
                {
                    AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    m_curAct = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                }

                return m_curAct;
            }
        }
        /// <summary>
        /// ��ȡ��ǰApplicationContext
        /// </summary>

        public static AndroidJavaObject ApplicationContext
        {
            get
            {
                return CurActivity.Call<AndroidJavaObject>("getApplicationContext");
            }
        }


        public static void OpenSystemMonitoring()
        {
            MainThreadQueue.Instance.ExecuteQueue.Enqueue(() =>
            {
                Debug.Log("[RayNeo]OpenSystemMonitoring");
                CurActivity.Call("openSystemMonitoring", 0l);
            });
        }

        public static void CloseSystemMonitoring()
        {
            MainThreadQueue.Instance.ExecuteQueue.Enqueue(() =>
            {
                Debug.Log("[RayNeo]CloseSystemMonitoring");
                CurActivity.Call("closeSystemMonitoring");
            });
        }


        /// <summary>
        /// ϵͳ�����Ϣ������ö��
        /// </summary>
        public class SystemMonitoringInfoType
        {

            //��ʾȫ��.
            public const long FULL = 0;

            //����
            public const long ELECTRIC = 1 << 0;
            //ƽ������
            public const long AVERAGE_ELECTRIC = 1 << 1;
            //��ǰ����
            public const long BATTERY_CAPACTIY = 1 << 2;

            //CPU����
            public const long CPU_TEMPERATURE = 1 << 3;

            //CPUʹ����
            public const long CPU_USAGE = 1 << 4;

            //�ڴ�ʹ��
            public const long MEM_USAGE = 1 << 5;

            //��ǰ����Ӧ��CPUʹ��
            public const long CUR_TOP_USAGE = 1 << 6;
        }
    }

    public class RayNeoApi
    {
        private static bool m_curAndroidDeviceInited = false;
        private static bool m_curAndroidDeviceIsMecury = false;
        private static float[] m_nineAxisNoientation = new float[4];

        /// <summary>
        /// ��ȡ��ǰ�豸�Ƿ��۾���
        /// </summary>
        /// <returns></returns>
        public static bool CurrentIsMecury()
        {
            if (m_curAndroidDeviceInited)
            {
                return m_curAndroidDeviceIsMecury;
            }

            m_curAndroidDeviceInited = true;
            if (SystemInfo.deviceModel.Equals("QUALCOMM kona for arm64") || SystemInfo.deviceModel.Equals("QUALCOMM ARGT78"))
            {
                m_curAndroidDeviceIsMecury = true;
            }
            else if (SystemInfo.deviceModel.Contains("RayNeo"))
            {
                m_curAndroidDeviceIsMecury = true;
            }
            else
            {
                m_curAndroidDeviceIsMecury = false;
            }
            return m_curAndroidDeviceIsMecury;
        }

        /// <summary>
        /// ��ȡƫ����
        /// </summary>
        /// <returns></returns>
        public static float GetAzimuth()
        {

#if UNITY_EDITOR
            return 0;
#else
            return Api.FXRUnity_NineAxisAzimuth();

#endif
        }


        /// <summary>
        /// ��ȡ�������Ƶ�3dof��ת��Ϣ
        /// </summary>
        /// <returns></returns>
        public static Quaternion GetNineAxisOrientation()
        {
            Api.GetNineAxisOrientation(m_nineAxisNoientation);
            return new Quaternion(m_nineAxisNoientation[0], m_nineAxisNoientation[1], m_nineAxisNoientation[2], m_nineAxisNoientation[3]);
        }
    }
}
