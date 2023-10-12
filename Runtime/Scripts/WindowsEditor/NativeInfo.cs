using RayNeo.Native;
using UnityEngine;

public class NativeInfo : MonoSingleton<NativeInfo>
{

    public WindowsMessager WindowsMessager;
    private HardwareInfo m_GlassInfo = new HardwareInfo();

    protected override void OnSingletonInit()
    {
        base.OnSingletonInit();
        Init();
    }


    private void Init()
    {
        WindowsMessager = new WindowsMessager(m_GlassInfo);
    }

    /// <summary>
    /// 获取眼镜的旋转值
    /// </summary>
    public Quaternion GetGlassesQualternion()
    {
        return WindowsMessager.GetGlassesQualternion(m_GlassInfo.DeltaGlassQuat);
    }
    /// <summary>
    /// 获取手机的旋转值
    /// </summary>
    public Quaternion GetMobileQualternion()
    {
        return WindowsMessager.GetMobileQualternion(m_GlassInfo.DeltaMobileQuat);
    }
}
