using FFalcon.XR.Runtime;
using RayNeo;
using RayNeo.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SampleSceneCtrl : MonoBehaviour
{

    public void OnBtnClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    public void OpenBatteryInfo()
    {
        AndroidActivity.OpenSystemMonitoring();
    }
    public void CloseBatteryInfo()
    {
        AndroidActivity.CloseSystemMonitoring();
    }


}
