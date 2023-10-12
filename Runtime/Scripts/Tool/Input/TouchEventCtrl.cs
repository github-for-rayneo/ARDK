using UnityEngine;
using HedgehogTeam.EasyTouch;
using System;
using System.Collections;
using RayNeo.Native;

namespace RayNeo.Tool
{
    /// <summary>
    /// 根据EasyTouch写的输入管理类
    /// </summary>
    public class TouchEventCtrl : MonoSingleton<TouchEventCtrl>
    {
        #region 数据定义
        private Gesture m_CurrentGesture;
        private EasyTouch.SwipeDirection m_LastSwipeDirection = EasyTouch.SwipeDirection.None;    //上一次滑动方向

        private Vector2 m_TouchDownPos = Vector2.zero;

        private float m_ClickDisLimit = 20f;    //避免两次点击的位置过远为误判
        private float m_TouchTime = 0;      //长按判断时间
        private float LONGPRESSTIME = 0.6f;   //定义长按时间.水星Launcher当前暂定长按呼出Docker时间为700ms

        private int TapCount = 0;

        private bool IsEndMonitor = true;       //判断当前点击类型(单击、双击、三击)是否结束判断
        private bool IsBeyond = true;       //判断两次点击的距离是否超过限制(超过限制则视为误判)

        private bool IsLongPress = false;      //判断是否为长按
        private bool IsLongPressExecute = false;//判断长按事件是否已执行(确认满足长按条件后只执行一次)
        private bool IsSwiping = false;         //是否处于滑动状态

        public bool IsEnable = true;           //输入模块响应开关


        public Gesture CurrentGestureure
        {
            get
            {
                return m_CurrentGesture;
            }
        }

        public Action OnSimpleTap;
        public Action OnDoubleTap;
        public Action OnTripleTap;

        public Action OnTouchStart;
        public Action OnTouchDown;
        public Action OnTouchUp;

        public Action OnSwipeLeftEnd;
        public Action OnSwipeRightEnd;

        public Action OnSwipeLeft;
        public Action OnSwipeRight;

        public Action OnLongTap;

        #endregion

        #region 生命周期

        protected override void Awake()
        {
            base.Awake();
            //将UI交互视为与其它交互一样
            EasyTouch.SetUICompatibily(false);
            //DontDestroyOnLoad(this.gameObject);
        }

        //protected override void OnDestroy()
        //{
        //}


        private void Update()
        {
            if (!IsEnable)
            {
                return;
            }

            m_CurrentGesture = EasyTouch.current;
            if (m_CurrentGesture == null)
            {
                return;
            }

            int fingerCount = m_CurrentGesture.touchCount;        //当前手指的数量
            int fingerIndex = m_CurrentGesture.fingerIndex;       //当前手指的索引
            if (fingerCount == 1)
            {
                switch (m_CurrentGesture.type)
                {
                    case EasyTouch.EvtType.On_TouchStart:
                        {
                            //OnTouchStart?.Invoke();

                            //长按逻辑初始化
                            m_TouchTime = 0;
                            IsLongPressExecute = false;
                            IsLongPress = false;
                            IsSwiping = false;
                            break;
                        }
                    case EasyTouch.EvtType.On_TouchDown:
                        {
                            //TouchDown在按下时每一帧都会执行.待需要时再将事件打开
                            //OnTouchDown?.Invoke();

                            //判断长按逻辑
                            m_TouchTime += Time.deltaTime;
                            if (m_TouchTime >= LONGPRESSTIME && !IsLongPressExecute)
                            {
                                IsLongPress = true;
                                IsLongPressExecute = true;
                                OnLongTap?.Invoke();
                            }
                            m_TouchDownPos = Input.mousePosition;
                            break;
                        }

                    case EasyTouch.EvtType.On_TouchUp:
                        {
                            //OnTouchUp?.Invoke();
                            if (!IsLongPress && !IsSwiping)
                            {
                                MonitorTouchCount();
                            }
                            IsSwiping = false;
                            IsLongPressExecute = false;
                            break;
                        }

                    case EasyTouch.EvtType.On_SwipeEnd:
                        {
                            //Swipend执行先于TouchUp,所以不在这里确认IsSwiping状态
                            if (m_CurrentGesture.swipe == EasyTouch.SwipeDirection.Left)
                            {
                                OnSwipeLeftEnd?.Invoke();
                            }
                            else if (m_CurrentGesture.swipe == EasyTouch.SwipeDirection.Right)
                            {
                                OnSwipeRightEnd?.Invoke();
                            }
                            break;
                        }

                    case EasyTouch.EvtType.On_Swipe:
                        {
                            //记录上一次滑动的方向
                            //滑动.在滑动时每一帧都会执行.待需要时再打开

                            //if (m_CurrentGesture.swipe == EasyTouch.SwipeDirection.Left)
                            //{
                            //    m_LastSwipeDirection = EasyTouch.SwipeDirection.Left;
                            //}
                            //else if (m_CurrentGesture.swipe == EasyTouch.SwipeDirection.Right)
                            //{
                            //    m_LastSwipeDirection = EasyTouch.SwipeDirection.Right;
                            //}

                            //if (m_LastSwipeDirection == EasyTouch.SwipeDirection.Left)
                            //{
                            //    OnSwipeLeft?.Invoke();
                            //}
                            //else if (m_LastSwipeDirection == EasyTouch.SwipeDirection.Right)
                            //{
                            //    OnSwipeRight?.Invoke();
                            //}

                            m_TouchTime = 0;
                            IsLongPressExecute = false;
                            IsSwiping = true;

                            break;
                        }

                    default: break;
                }
            }
        }

        #endregion

        #region 点击检测
        private void MonitorTouchCount()
        {
            IsBeyond = (Vector2.Distance(Input.mousePosition, m_TouchDownPos) > m_ClickDisLimit);
            TapCount++;
            //上一次的事件是否已经执行完毕，也就是判断是否为新一轮
            if (!IsEndMonitor)
            {
                return;
            }
            IsEndMonitor = false;

            //初始化定时器，400毫秒后执行预定方法.300毫秒可能来不及捕获到第三次点击
            //如果需要定义更多次数的连击，方法类似.
            Invoke("MonitorTimer", 0.4f);
        }

        private void MonitorTimer()
        {
            if (!IsBeyond)
            {
                if (TapCount == 1)
                {
                    OnSimpleTap?.Invoke();
                }
                else if (TapCount == 2)
                {
                    OnDoubleTap?.Invoke();
                }
                else if (TapCount == 3)
                {
                    OnTripleTap?.Invoke();
                }
            }
            //定时调用结束，重置标识
            IsEndMonitor = true;
            TapCount = 0;
        }

        //private void OnApplicationPause(bool IsPause)
        //{
        //    if (IsPause)
        //    {
        //        Debug.Log("Autumnscity:后台");
        //        IsEnable = false;
        //    }
        //    else
        //    {
        //        Debug.Log("Autumnscity:前台");
        //    }
        //}
        #endregion
    }
}