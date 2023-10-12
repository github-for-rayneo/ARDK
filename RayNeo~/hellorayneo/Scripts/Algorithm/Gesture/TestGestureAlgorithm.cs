using RayNeo;
using RayNeo.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGestureAlgorithm : MonoBehaviour
{
    #region 数据定义

    private int Pointer = 0;
    private int Five = 0;
    private int PointerAndStill = 0;

    public Text FiveNum;
    public Text PointerNum;
    public Text CurrentSkeletonContent;
    //捕获手势
    public Text StateCheck;
    //是否静态
    public Text DynamicOrStill;
    public Text StillAndPointer;

    public bool IsStill { get; set; }
    #endregion

    #region 生命周期

    private void Awake()
    {
        GestureManager.Ins.Start();
        GestureManager.Ins.HandStaticStateChange += OnIsStillChanged;
        GestureManager.Ins.GestureTypeCallback += GestureTypeCallback;
    }

    private void Update()
    {
        StateCheck.text = GestureManager.Ins.IsCaptureGesture().ToString();
    }

    private void OnDestroy()
    {
        GestureManager.Ins.HandStaticStateChange -= OnIsStillChanged;
        GestureManager.Ins.GestureTypeCallback -= GestureTypeCallback;
        GestureManager.Ins.Stop();
    }
    #endregion

    #region 手势识别
    private void GetCurrentGesture(GestureType GestureType)
    {
        switch (GestureType)
        {
            case GestureType.Five:
                {
                    Five++;
                    FiveNum.text = Five.ToString();
                    break;
                }
            case GestureType.Pointer:
                {
                    Pointer++;
                    PointerNum.text = Pointer.ToString();
                    break;
                }
            case GestureType.Nothing:
            default:
                {
                    break;
                }
        }
    }

    private void OnIsStillChanged(bool isStill)
    {
        IsStill = isStill;
        if (isStill)
        {
            DynamicOrStill.text = "静态";
        }
        else
        {
            DynamicOrStill.text = "动态";
        }
    }
    private void GestureTypeCallback(GestureType GestureType)
    {
        GetCurrentGesture(GestureType);
        //Debug.Log("[MercuryX2]当前手势类型|" + GestureType);
        if (IsStill && GestureType == GestureType.Pointer)
        {
            PointerAndStill++;
            StillAndPointer.text = PointerAndStill.ToString();
        }
    }

    #endregion
}
