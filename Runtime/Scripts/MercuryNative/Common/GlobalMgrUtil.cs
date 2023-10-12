using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace RayNeo.Native
{
    /// <summary>
    /// 全局管理类
    /// </summary>
    public class GlobalMgrUtil : MonoSingleton<GlobalMgrUtil>
    {

        public string PREFIX = "[RayNeoX2]";

        #region 轮询网络

        public Action<bool> IsInternetUnAvailable;

        private void InternetDetection()
        {
            bool CurrentInternetStatus = Application.internetReachability == NetworkReachability.NotReachable;
            IsInternetUnAvailable?.Invoke(CurrentInternetStatus);
        }

        /// <summary>
        /// 启动网络检测
        /// 每隔2秒检测一次
        /// </summary>
        public void StartInternetDetection()
        {
            InvokeRepeating("InternetDetection", 0, 2);
        }

        /// <summary>
        /// 关闭网络监测
        /// </summary>
        public void StopInternetDetection()
        {
            Debug.Log("[RayNeoX2]:停止网络状态轮询");
            if (IsInvoking("InternetDetection"))
            {
                CancelInvoke("InternetDetection");
            }
        }

        #endregion

        #region 外部轮询

        private Action ExternalPoll;
        private void Polling()
        {
            ExternalPoll?.Invoke();
        }

        public void StartPoll(Action ExternalPoll, float Interval)
        {
            this.ExternalPoll = ExternalPoll;
            InvokeRepeating("Polling", 0, Interval);
        }

        public void StopPoll()
        {
            if (IsInvoking("Polling"))
            {
                CancelInvoke("Polling");
            }
        }

        #endregion

        #region 加载图集

        public SpriteAtlas LoadSpriteFromAssetBundle(string AssetPath, string AtlasName)
        {
            AssetBundle Bundle = AssetBundle.LoadFromFile(AssetPath);
            //从AssetBundle包中加载图集
            SpriteAtlas Atlas = Bundle.LoadAsset<SpriteAtlas>(AtlasName);
            return Atlas;
        }
        #endregion


    }
}