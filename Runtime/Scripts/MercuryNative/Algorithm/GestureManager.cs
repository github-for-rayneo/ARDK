
using RayNeo.Native;
using System;
using UnityEngine;

namespace RayNeo
{
    public enum GestureType
    {
        Nothing,
        Five,
        Pointer
    }
    public class GestureManager
    {

        private static GestureManager ins = new GestureManager();
        public static GestureManager Ins
        {
            get
            {
                return ins;
            }
        }

        private AndroidJavaObject m_javaGestureCtrl;
        private AndroidJavaObject m_javancnnMPCtrl;
        public Action<GestureType> GestureTypeCallback;

        public Action<bool> HandStaticStateChange;
        public Action<bool, float, float[], float[]> OnPalmInfoChanged;

        private bool m_isHandStatic = false;//手是否静止了.


        public void Start(int w = 640, int h = 480, int f = 20)
        {
#if UNITY_EDITOR
            return;
#endif
            Debug.Log("[GestureManager.Init.RayNeoX2]开启算法");
            if (m_javaGestureCtrl == null)
            {
                m_javaGestureCtrl = new AndroidJavaObject("com.tcl.unity.unityadapter.algorithm.gesture.GestureController").CallStatic<AndroidJavaObject>("getInstance");
                m_javancnnMPCtrl = m_javaGestureCtrl.Call<AndroidJavaObject>("getMediapipehand");
                InitPalmListener();
                InitMotionListener();
            }
            AndroidJavaObject uActivity = GetUnityActivity();
            m_javaGestureCtrl.Call("startAlgorithm", uActivity, GetCacheDir(), uActivity.Call<AndroidJavaObject>("getAssets"), w, h, f);
            GlobalMgrUtil.Instance.StartPoll(GetCurrentGesture, 0.3f);

            //GestureRecCtrl.InitAlgorithm();
            //StartCheckGestureType();
        }

        private void InitPalmListener()
        {
            HandsInfoListener listener = new HandsInfoListener(OnPalmInfo);
            m_javancnnMPCtrl.Call("setHandsInfoListener", listener);
        }

        /// <summary>
        /// 调用手势动静接口
        /// </summary>
        private void InitMotionListener()
        {
            SetMotionListener Listener = new SetMotionListener(OnMotionStaticStateChange);
            m_javancnnMPCtrl.Call("setMotionListener", Listener);
        }

        public void Stop()
        {
            if (m_javaGestureCtrl == null)
            {
                return;
            }
            GlobalMgrUtil.Instance.StopPoll();
            m_javaGestureCtrl.Call("stopAlgorithm");
            Debug.Log("[GestureManager.Stop.RayNeoX2]:算法模块关闭");

        }
        public void GetCurrentGesture()
        {
            GestureType CurrentGestureType = GestureType.Nothing;

            string Getsture = m_javancnnMPCtrl.Call<string>("getGesture");
            if (string.IsNullOrEmpty(Getsture))
            {
                GestureTypeCallback?.Invoke(GestureType.Nothing);
                return;
            }
            if (Getsture.Equals("Five"))
            {
                CurrentGestureType = GestureType.Five;

            }
            else if (Getsture.Equals("Pointer"))
            {
                CurrentGestureType = GestureType.Pointer;

            }
            else
            {
                CurrentGestureType = GestureType.Nothing;
            }

            GestureTypeCallback?.Invoke(CurrentGestureType);
        }

        public bool IsCaptureGesture()
        {
            if (m_javancnnMPCtrl == null)
            {
                return false;
            }
            bool isGetGesture = m_javancnnMPCtrl.Call<bool>("checkState");
            return isGetGesture;
        }
        /// <summary>
        /// 获取手势骨节点坐标数组
        /// </summary>
        /// <returns></returns>
        public float[] GetSkeletonCoordinates()
        {
            float[] SkeletonCoordinates = m_javancnnMPCtrl.Call<float[]>("getSkeletonCoordinate");
            if (SkeletonCoordinates != null)
            {
                return SkeletonCoordinates;
            }
            return null;
        }
        private void OnPalmInfo(bool isRecognized, float handSize, float[] position, float[] normal)
        {
            OnPalmInfoChanged?.Invoke(isRecognized, handSize, position, normal);
        }

        private void OnMotionStaticStateChange(bool isStatic)
        {
            m_isHandStatic = isStatic;
            HandStaticStateChange?.Invoke(m_isHandStatic);
        }

        ///// <summary>
        ///// 设置手势动静识别时间判定阈值
        ///// </summary>
        ///// <param name="TimeScale"></param>
        //public void SetTimeThreshold(float TimeScale)
        //{
        //    m_javancnnMPCtrl.Call("setTimeThreshold", TimeScale);
        //}
        /// <summary>
        /// 设置手势动静识别像素变化幅度阈值
        /// </summary>
        /// <param name="TimeScale"></param>
        public void SetPixelThreshold(float TimeScale)
        {
            m_javancnnMPCtrl.Call("setPixelThreshold", TimeScale);
        }


        public AndroidJavaObject GetJavaMP()
        {
            return m_javancnnMPCtrl;
        }


        #region 动静识别接口
        public sealed class SetMotionListener : AndroidJavaProxy
        {
            private Action<bool> m_Interface;

            public SetMotionListener(Action<bool> Interface) : base("com.ncnn.mediapipehand.NcnnMediapipeHand$handsMotionListener")
            {
                Debug.Log("[SetMotionListener.RayNeoX2]:SetMotionListener");
                m_Interface = Interface;
            }
            public void onMotionChanged(bool IsStill)
            {
                MainThreadQueue.Instance.ExecuteQueue.Enqueue(() =>
                {
                    m_Interface.Invoke(IsStill);
                });
            }
        }

        #endregion

        #region 手掌识别接口

        public sealed class HandsInfoListener : AndroidJavaProxy
        {
            private Action<bool, float, float[], float[]> m_onHandsInfoCallBack;

            public HandsInfoListener(Action<bool, float, float[], float[]> onHandsInfoCallBack)
                : base("com.ncnn.mediapipehand.NcnnMediapipeHand$handsInfoListener")
            {
                m_onHandsInfoCallBack = onHandsInfoCallBack;
            }

            public void onHandsInfo(bool isRecognized, float handSize, float[] position, float[] normal)
            {
                MainThreadQueue.Instance.ExecuteQueue.Enqueue(() =>
                {
                    m_onHandsInfoCallBack?.Invoke(isRecognized, handSize, position, normal);
                });
            }
        }

        #endregion



        private AndroidJavaObject GetUnityActivity()
        {
            AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject UnityActivity = UnityClass.GetStatic<AndroidJavaObject>("currentActivity");
            return UnityActivity;
        }

        public string GetCacheDir()
        {
            using (var CacheDir = GetUnityActivity().Call<AndroidJavaObject>("getCacheDir"))
            {
                if (CacheDir == null)
                {
                    return string.Empty;
                }
                string path = CacheDir.Call<string>("getAbsolutePath");
                Debug.Log("[GestureManager.GestureManagerRayNeoX2]:cache path|" + path);
                return path;
            }
        }
    }
}