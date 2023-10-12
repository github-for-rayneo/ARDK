using UnityEngine;

/// <summary>
/// 获取设备信息
/// </summary>
public class GetDeviceInfo : MonoBehaviour
{

    private void Start()
    {
        GetDeviceInformation();
    }

    private void GetDeviceInformation()
    {
       Debug.Log("[RayNeoX2]:读取设备的详细信息");
       Debug.Log("[RayNeoX2]:设备模型:" + SystemInfo.deviceModel);
       Debug.Log("[RayNeoX2]:设备名称:" + SystemInfo.deviceName);
       Debug.Log("[RayNeoX2]:设备类型:" + SystemInfo.deviceType.ToString());
       Debug.Log("[RayNeoX2]:设备唯一标识符:" + SystemInfo.deviceUniqueIdentifier);
       Debug.Log("[RayNeoX2]:是否支持纹理复制:" + SystemInfo.copyTextureSupport.ToString());
       Debug.Log("[RayNeoX2]:显卡ID:" + SystemInfo.graphicsDeviceID.ToString());
       Debug.Log("[RayNeoX2]:显卡名称:" + SystemInfo.graphicsDeviceName);
       Debug.Log("[RayNeoX2]:显卡类型:" + SystemInfo.graphicsDeviceType.ToString());
       Debug.Log("[RayNeoX2]:显卡供应商:" + SystemInfo.graphicsDeviceVendor);
       Debug.Log("[RayNeoX2]:显卡供应商ID:" + SystemInfo.graphicsDeviceVendorID.ToString());
       Debug.Log("[RayNeoX2]:显卡版本号:" + SystemInfo.graphicsDeviceVersion);
       Debug.Log("[RayNeoX2]:显存大小（单位：MB）:" + SystemInfo.graphicsMemorySize);
       Debug.Log("[RayNeoX2]:显卡是否支持多线程渲染:" + SystemInfo.graphicsMultiThreaded.ToString());
       Debug.Log("[RayNeoX2]:支持的渲染目标数量:" + SystemInfo.supportedRenderTargetCount.ToString());
       Debug.Log("[RayNeoX2]:系统内存大小(单位：MB):" + SystemInfo.systemMemorySize.ToString());
       Debug.Log("[RayNeoX2]:操作系统:" + SystemInfo.operatingSystem);
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("设备的详细信息");
        GUILayout.Label("设备模型:" + SystemInfo.deviceModel);
        GUILayout.Label("设备名称:" + SystemInfo.deviceName);
        GUILayout.Label("设备类型:" + SystemInfo.deviceType.ToString());
        GUILayout.Label("设备唯一标识符:" + SystemInfo.deviceUniqueIdentifier);
        GUILayout.Label("是否支持纹理复制:" + SystemInfo.copyTextureSupport.ToString());
        GUILayout.Label("显卡ID:" + SystemInfo.graphicsDeviceID.ToString());
        GUILayout.Label("显卡名称:" + SystemInfo.graphicsDeviceName);
        GUILayout.Label("显卡类型:" + SystemInfo.graphicsDeviceType.ToString());
        GUILayout.Label("显卡供应商:" + SystemInfo.graphicsDeviceVendor);
        GUILayout.Label("显卡供应商ID:" + SystemInfo.graphicsDeviceVendorID.ToString());
        GUILayout.Label("显卡版本号:" + SystemInfo.graphicsDeviceVersion);
        GUILayout.Label("显存大小（单位：MB）:" + SystemInfo.graphicsMemorySize);
        GUILayout.Label("显卡是否支持多线程渲染:" + SystemInfo.graphicsMultiThreaded.ToString());
        GUILayout.Label("支持的渲染目标数量:" + SystemInfo.supportedRenderTargetCount.ToString());
        GUILayout.Label("系统内存大小(单位：MB):" + SystemInfo.systemMemorySize.ToString());
        GUILayout.Label("操作系统:" + SystemInfo.operatingSystem);
    }


}
