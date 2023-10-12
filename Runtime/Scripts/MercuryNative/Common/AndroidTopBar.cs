using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FfalconXR.Native
{
    public class AndroidTopBar
    {
        public static  AndroidTopBar mBar = new AndroidTopBar();

        private AndroidJavaObject mTopBar;
        public static AndroidTopBar Ins
        {
            get { return mBar; }
        }
        public void Init()
        {
            if(mTopBar != null)
            {
                //初始化过了.
                return;
            }
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            mTopBar = currentActivity.Call<AndroidJavaObject>("getTopBarCtrl");


        }


        public void ShowBackBtn()
        {
            Init();
            mTopBar.Call("showBackBtn");


        }

        public void HideBackBtn()
        {
            Init();
            mTopBar.Call("hideBackBtn");

        }

        public void ShowResetBtn()
        {
            Init();
            mTopBar.Call("showResetBtn");
        }

        public void HideResetBtn()
        {
            Init();
            mTopBar.Call("hideResetBtn");
        }
    }
}