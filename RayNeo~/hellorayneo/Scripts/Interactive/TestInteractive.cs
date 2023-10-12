using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RayNeo.Native;

public class TestInteractive : MonoBehaviour
{
    [SerializeField]
    private GameObject TestImage;
    [SerializeField]
    private GameObject TestCube;

    private void Start()
    {
        MercuryEventTriggerListener.Get(TestImage).OnPointEnter += OnPointerEnterImage;
        MercuryEventTriggerListener.Get(TestImage).OnPointExit  += OnPointerExitImage;
        MercuryEventTriggerListener.Get(TestImage).OnPointClick += OnPointerClickImage;

        GlobalMgrUtil.Instance.StartInternetDetection();
        //GlobalMgrUtil.Instance.IsInternetUnAvailable += IsInternetUnAvailable;
    }

    private void OnPointerEnterImage(GameObject go)
    {
        Debug.Log("[RayNeoX2] " + go.name + ":OnPointerEnter");
    }

    private void OnPointerExitImage(GameObject go)
    {
        Debug.Log("[RayNeoX2] " + go.name + ":OnPointerExit");
    }

    private void OnPointerClickImage(GameObject go)
    {
        Debug.Log("[RayNeoX2] " + go.name + ":OnPointerClick");
    }


    public void OnPointerEnter()
    {
        Debug.Log("[RayNeoX2] Cube:OnPointerEnter");
    }

    public void OnPointerExit()
    {
        Debug.Log("[RayNeoX2] Cube:OnPointerExit");
    }

    public void OnPointerClick()
    {
        Debug.Log("[RayNeoX2] Cube:OnPointerClick");
    }

    private void IsInternetUnAvailable(bool IsAvilable)
    {

        if (IsAvilable)
        {
            Debug.Log("[RayNeoX2]:当前网络异常");
        }
        else
        { 
            Debug.Log("[RayNeoX2]:当前网络正常");
        }
    }

}
