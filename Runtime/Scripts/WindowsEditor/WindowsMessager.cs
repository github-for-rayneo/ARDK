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
        /// 获取眼镜的四元数
        /// </summary>
        public Quaternion GetGlassesQualternion(Quaternion deltaQuat)
        {
            return m_windowsCameraQuaternion;
        }

        /// <summary>
        /// 获取手机的角度
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
    /// 设备状态数据
    /// </summary>
    public class HardwareInfo
    {
        /// <summary>
        /// 分情况：
        /// 1.我是service
        ///     该标记正常运作，并且会主动切3d，重新设置该值
        /// 2.我是client
        ///     该标记正常获取，但是主动切3d接口调用后，不会有反馈。
        ///     如果眼镜不是3d的  client也没有权力去切换。标记仍然是对的
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