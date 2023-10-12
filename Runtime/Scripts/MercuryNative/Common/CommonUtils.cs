using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RayNeo.Native
{
    /// <summary>
    /// 全局工具类
    /// </summary>
    public class CommonUtils
    {
        #region 时间戳常用工具
        /// <summary>
        /// 时间戳Timestamp转换成日期   今天  昨天
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static string SetTimeType(string timeStamp, bool isNeedAllTime = false)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(string.Format("{0}0000000", timeStamp));
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            if (isNeedAllTime)
            {
                return dtStart.Add(toNow).ToString("yyyy年MM月dd日 HH:mm");
            }
            else
            {
                long ms = GetMilliseconds(DateTime.Now) - GetMilliseconds(dtStart.Add(toNow));
                if (ms <= 86400000)
                {
                    return dtStart.Add(toNow).ToString("HH:mm");
                }
                else if (ms > 86400000 && ms < 172800000)
                {
                    return "昨天";
                }
                else
                {
                    return dtStart.Add(toNow).ToString("MM月dd日");
                }
            }
        }

        /// <summary>
        /// 时间转换为毫秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long GetMilliseconds(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// 获取当前本地时间戳
        /// </summary>
        /// <returns></returns>      
        public static long GetCurrentTimeUnix()
        {
            TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
            long t = (long)cha.TotalSeconds;
            return t;
        }

        #endregion

        #region 字符串常用工具
        /// <summary>
        /// 首行缩进2字符
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string IndentString(string msg)
        {
            return string.Format("\u3000\u3000{0}", msg);
        }


        /// <summary>
        /// 多余字符省略号处理
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static string SetTextWithEllipsis(string txt, int startIndex, int endIndex)
        {
            if (txt.Length < endIndex)
            {
                return txt;
            }
            txt = txt.Substring(startIndex, endIndex);
            return string.Format("{0}...", txt);
        }

        /// <summary>
        /// 在sourse获取startstr 和endstr中间的字符串
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="startstr"></param>
        /// <param name="endstr"></param>
        /// <returns></returns>
        public static string MidStrEx(string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;
            try
            {
                startindex = sourse.IndexOf(startstr);
                if (startindex == -1)
                {
                    return result;
                }
                string tmpstr = sourse.Substring(startindex + startstr.Length);
                endindex = tmpstr.IndexOf(endstr);
                if (endindex == -1)
                {
                    return result;
                }
                result = tmpstr.Remove(endindex);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 文字长度截取(超出部分使用...代替)
        /// </summary>
        public static string InterceptionLength(string str, int minLength)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (str.Length <= minLength) return str;

            string temp = null;

            float count = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].Equals('@') || str[i].Equals('￥') || str[i].Equals('%'))
                {
                    count += 1;
                }
                else
                {
                    if (str[i] > 127) count += 1;
                    else count += 0.5f;
                }

                if (count - minLength > -0.5f)
                {
                    break;
                }
                else temp += str[i];
            }
            return temp += "...";
        }

        #endregion

        #region 网络状态检测工具
        /// <summary>
        /// 网络不可用
        /// </summary>
        public static bool NetUnAvailable
        {
            get
            {
                return Application.internetReachability == NetworkReachability.NotReachable;
            }
        }


    
        #endregion

        #region 物体方位判断工具

        /// <summary>
        /// 判断目标基于位置的前后方位
        /// <returns>true:前.false:后</returns>
        public static bool IsOnForward(Transform BaseTrans, Vector3 TargetPos)
        {
            return GetForwardStatus(BaseTrans, TargetPos) >= 0 ? true : false;
        }

        /// <summary>
        /// 判断目标基于位置的左右方位
        /// </summary>
        /// <returns>true:右.false:左</returns>
        public static bool IsOnRight(Transform BaseTrans, Vector3 TargetPos)
        {
            return GetRightStatus(BaseTrans, TargetPos) >= 0 ? true : false;
        }

        // 大于0为前
        // TargetPos为0,结果会出错(最好是相对位置)
        public static float GetForwardStatus(Transform BaseTrans, Vector3 TargetPos)
        {
            return Vector3.Dot(BaseTrans.forward, TargetPos - BaseTrans.position);
        }

        // 大于0为右
        // TargetPos为0,结果会出错(最好是相对位置)
        public static float GetRightStatus(Transform BaseTrans, Vector3 TargetPos)
        {
            return Vector3.Cross(BaseTrans.forward, TargetPos - BaseTrans.position).y;
        }

        public static float GetViewportPoint()
        {
            return 0;
        }
        #endregion

        #region 日志常用工具
        public static void SubLog(string m_Message)
        {
            //信息太长,分段打印        
            int m_OutputStringLength = 500;        //大于500时       

            while (m_Message.Length > m_OutputStringLength)
            {
                Debug.Log(m_Message.Substring(0, m_OutputStringLength));
                m_Message = m_Message.Substring(m_OutputStringLength);
            }
            //剩余部分    
            Debug.Log(m_Message);
        }
        #endregion
    }
}