using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayNeo.Native
{
    public class WindowsMessager
    {
        private HardwareInfo m_GlassInfo;

        public Quaternion m_windowsMouseQuaternion = Quaternion.identity;
        public Quaternion m_windowsCameraQuaternion = Quaternion.identity;

        /// <summary>
        /// ��ȡ�۾�����Ԫ��
        /// </summary>
        public Quaternion GetGlassesQualternion(Quaternion deltaQuat)
        {
            return m_windowsCameraQuaternion;
        }

        /// <summary>
        /// ��ȡ�ֻ��ĽǶ�
        /// </summary>
        public Quaternion GetMobileQualternion(Quaternion mobileQuat)
        {
            return m_windowsMouseQuaternion;
        }

        public WindowsMessager(HardwareInfo hardwareInfo)
        {
            m_GlassInfo = hardwareInfo;
        }
    }
    /// <summary>
    /// �豸״̬����
    /// </summary>
    public class HardwareInfo
    {
        /// <summary>
        /// �������
        /// 1.����service
        ///     �ñ���������������һ�������3d���������ø�ֵ
        /// 2.����client
        ///     �ñ��������ȡ������������3d�ӿڵ��ú󣬲����з�����
        ///     ����۾�����3d��  clientҲû��Ȩ��ȥ�л��������Ȼ�ǶԵ�
        /// </summary>
        public bool Is3DMode { get; set; }

        public Quaternion DeltaGlassQuat;

        public Quaternion DeltaMobileQuat;

        public HardwareInfo()
        {
            DeltaGlassQuat = Quaternion.identity;
            DeltaMobileQuat = Quaternion.identity;
            Is3DMode = false;
        }
    }
}