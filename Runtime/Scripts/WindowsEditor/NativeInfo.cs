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
    /// ��ȡ�۾�����תֵ
    /// </summary>
    public Quaternion GetGlassesQualternion()
    {
        return WindowsMessager.GetGlassesQualternion(m_GlassInfo.DeltaGlassQuat);
    }
    /// <summary>
    /// ��ȡ�ֻ�����תֵ
    /// </summary>
    public Quaternion GetMobileQualternion()
    {
        return WindowsMessager.GetMobileQualternion(m_GlassInfo.DeltaMobileQuat);
    }
}
