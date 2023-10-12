using UnityEngine;
using HedgehogTeam.EasyTouch;
using System;
using System.Collections;
using RayNeo.Native;

namespace RayNeo.Tool
{
    /// <summary>
    /// ����EasyTouchд�����������
    /// </summary>
    public class TouchEventCtrl : MonoSingleton<TouchEventCtrl>
    {
        #region ���ݶ���
        private Gesture m_CurrentGesture;
        private EasyTouch.SwipeDirection m_LastSwipeDirection = EasyTouch.SwipeDirection.None;    //��һ�λ�������

        private Vector2 m_TouchDownPos = Vector2.zero;

        private float m_ClickDisLimit = 20f;    //�������ε����λ�ù�ԶΪ����
        private float m_TouchTime = 0;      //�����ж�ʱ��
        private float LONGPRESSTIME = 0.6f;   //���峤��ʱ��.ˮ��Launcher��ǰ�ݶ���������Dockerʱ��Ϊ700ms

        private int TapCount = 0;

        private bool IsEndMonitor = true;       //�жϵ�ǰ�������(������˫��������)�Ƿ�����ж�
        private bool IsBeyond = true;       //�ж����ε���ľ����Ƿ񳬹�����(������������Ϊ����)

        private bool IsLongPress = false;      //�ж��Ƿ�Ϊ����
        private bool IsLongPressExecute = false;//�жϳ����¼��Ƿ���ִ��(ȷ�����㳤��������ִֻ��һ��)
        private bool IsSwiping = false;         //�Ƿ��ڻ���״̬

        public bool IsEnable = true;           //����ģ����Ӧ����


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

        #region ��������

        protected override void Awake()
        {
            base.Awake();
            //��UI������Ϊ����������һ��
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

            int fingerCount = m_CurrentGesture.touchCount;        //��ǰ��ָ������
            int fingerIndex = m_CurrentGesture.fingerIndex;       //��ǰ��ָ������
            if (fingerCount == 1)
            {
                switch (m_CurrentGesture.type)
                {
                    case EasyTouch.EvtType.On_TouchStart:
                        {
                            //OnTouchStart?.Invoke();

                            //�����߼���ʼ��
                            m_TouchTime = 0;
                            IsLongPressExecute = false;
                            IsLongPress = false;
                            IsSwiping = false;
                            break;
                        }
                    case EasyTouch.EvtType.On_TouchDown:
                        {
                            //TouchDown�ڰ���ʱÿһ֡����ִ��.����Ҫʱ�ٽ��¼���
                            //OnTouchDown?.Invoke();

                            //�жϳ����߼�
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
                            //Swipendִ������TouchUp,���Բ�������ȷ��IsSwiping״̬
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
                            //��¼��һ�λ����ķ���
                            //����.�ڻ���ʱÿһ֡����ִ��.����Ҫʱ�ٴ�

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

        #region ������
        private void MonitorTouchCount()
        {
            IsBeyond = (Vector2.Distance(Input.mousePosition, m_TouchDownPos) > m_ClickDisLimit);
            TapCount++;
            //��һ�ε��¼��Ƿ��Ѿ�ִ����ϣ�Ҳ�����ж��Ƿ�Ϊ��һ��
            if (!IsEndMonitor)
            {
                return;
            }
            IsEndMonitor = false;

            //��ʼ����ʱ����400�����ִ��Ԥ������.300����������������񵽵����ε��
            //�����Ҫ��������������������������.
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
            //��ʱ���ý��������ñ�ʶ
            IsEndMonitor = true;
            TapCount = 0;
        }

        //private void OnApplicationPause(bool IsPause)
        //{
        //    if (IsPause)
        //    {
        //        Debug.Log("Autumnscity:��̨");
        //        IsEnable = false;
        //    }
        //    else
        //    {
        //        Debug.Log("Autumnscity:ǰ̨");
        //    }
        //}
        #endregion
    }
}