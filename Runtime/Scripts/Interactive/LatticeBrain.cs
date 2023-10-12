using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RayNeo.Native;
using RayNeo.Tool;
using System;

/// <summary>
/// 晶格操作按钮
/// </summary>
public class LatticeBrain : MonoBehaviour
{
    private static LatticeBrain m_brain;

    public static LatticeBrain Brain
    {
        get
        {
            if (m_brain == null)
            {
                m_brain = FindObjectOfType<LatticeBrain>();
                if (m_brain == null)
                {
                    m_brain = new GameObject("LatticeBrain").AddComponent<LatticeBrain>();
                    m_brain.transform.parent = null;

                }
            }
            return m_brain;

        }
    }
    public Action OnDoubleTap;

    private Dictionary<int, LatticeSelectInfo> levelInButtons = new Dictionary<int, LatticeSelectInfo>();
    //private 

    private LatticeSelectInfo m_curInteractionInfo;


    /// <summary>
    /// 聚焦该层级.
    /// </summary>
    /// <param name="level"></param>
    public static void FocusLevel(int level)
    {
        if (Brain.levelInButtons.TryGetValue(level, out LatticeSelectInfo info))
        {
            Brain.m_curInteractionInfo = info;
        }
    }
    public static void RegButton(LatticeButton btn)
    {

        LatticeSelectInfo lsi;
        if (!Brain.levelInButtons.TryGetValue(btn.level, out lsi))
        {
            lsi = Brain.levelInButtons[btn.level] = new LatticeSelectInfo();
        }
        lsi.AddButton(btn);
    }
    public static void CleanLevel(int level)
    {

        if (Brain.levelInButtons.TryGetValue(level, out LatticeSelectInfo lsi))
        {
            Brain.levelInButtons.Remove(level);
        }
    }

    public static void RemoveButton(LatticeButton btn)
    {

        if (Brain.levelInButtons.TryGetValue(btn.level, out LatticeSelectInfo lsi))
        {
            lsi.RemoveButton(btn);
            if (!lsi.HasBtn)
            {
                Brain.levelInButtons.Remove(btn.level);
            }
        }
    }

    public static void SelectButton(LatticeButton btn, bool focusLevel = false)
    {

        foreach (var item in Brain.levelInButtons)
        {
            if (item.Key == btn.level)
            {
                item.Value.SelectButton(btn);
                if (focusLevel)
                {
                    Brain.m_curInteractionInfo = item.Value;
                }
                break;
            }
        }
    }


    private void Awake()
    {
        TouchEventCtrl.Instance.OnDoubleTap += OnDoubleTapCall;
        TouchEventCtrl.Instance.OnSimpleTap += OnSimpleTap;
        TouchEventCtrl.Instance.OnSwipeLeftEnd += OnSwipeLeftEnd;
        TouchEventCtrl.Instance.OnSwipeRightEnd += OnSwipeRightEnd;
    }


    private void OnDisable()
    {
        m_brain = null;
    }
    private void OnDestroy()
    {
        TouchEventCtrl.Instance.OnDoubleTap -= OnDoubleTapCall;
        TouchEventCtrl.Instance.OnSimpleTap -= OnSimpleTap;
        TouchEventCtrl.Instance.OnSwipeLeftEnd -= OnSwipeLeftEnd;
        TouchEventCtrl.Instance.OnSwipeRightEnd -= OnSwipeRightEnd;
    }


    private void OnSwipeLeftEnd()
    {
        if (m_curInteractionInfo == null)
        {
            return;
        }
        m_curInteractionInfo.SelectPreview();

    }
    private void OnSwipeRightEnd()
    {
        if (m_curInteractionInfo == null)
        {
            return;
        }
        m_curInteractionInfo.SelectNext();
    }

    private void OnDoubleTapCall()
    {
        if (m_curInteractionInfo == null)
        {
            return;
        }
        OnDoubleTap?.Invoke();
    }

    private void OnSimpleTap()
    {
        if (m_curInteractionInfo == null)
        {
            return;
        }

        m_curInteractionInfo.OnBtnClick();
    }


    private class SameLevelBtnCompair : IComparer<LatticeButton>
    {
        public int Compare(LatticeButton x, LatticeButton y)
        {
            return x.order - y.order;
        }
    }

    private class LatticeSelectInfo
    {
        private bool m_btnSelected = false;//有没有按钮选择着
        public LatticeButton mono;//有可能被销毁了.

        public List<LatticeButton> btns = new List<LatticeButton>();

        private SameLevelBtnCompair m_compair = new SameLevelBtnCompair();


        public bool HasBtn { get { return btns.Count > 0; } }

        public void AddButton(LatticeButton btn)
        {
            for (int i = 0; i < btns.Count; i++)
            {
                if (btns[i] == btn)
                {
                    return;//重复
                }
            }
            btns.Add(btn);
            btns.Sort(m_compair);
        }

        public void RemoveButton(LatticeButton lb)
        {
            if (!btns.Contains(lb))
            {
                return;
            }
            if (mono == lb)
            {
                //相等
                m_btnSelected = false;
            }

            btns.Remove(lb);
            lb.MonoUnFocus();

        }

        public void SelectButton(LatticeButton lb)
        {
            if (!btns.Contains(lb))
            {
                return;
            }
            if(mono == lb)
            {
                return;
            }
            lb.MonoFocus();
            if (!m_btnSelected)
            {
                //之前没有选中的。
                mono = lb;
                m_btnSelected = true;
                return;
            }
            if (mono != null)
            {
                mono.MonoUnFocus();//上一个按钮取消聚焦。
            }
            mono = lb;

        }


        public void SelectNext()
        {

            if (!OnBtnModifyCheck())
            {
                return;
            }

            for (int i = 0; i < btns.Count; i++)
            {
                if (btns[i] == mono)
                {
                    if (i == (btns.Count - 1))
                    {
                        SelectButton(btns[0]);
                    }
                    else
                    {
                        SelectButton(btns[i + 1]);
                    }
                    break;
                }
            }
        }

        public void SelectPreview()
        {
            if (!OnBtnModifyCheck())
            {
                return;
            }

            for (int i = 0; i < btns.Count; i++)
            {
                if (btns[i] == mono)
                {
                    if (i == 0)
                    {
                        SelectButton(btns[btns.Count - 1]);
                    }
                    else
                    {
                        SelectButton(btns[i - 1]);
                    }
                    break;
                }
            }

        }

        private bool OnBtnModifyCheck()
        {
            if (btns.Count <= 0)
            {
                return false;
            }

            if (mono == null || !btns.Contains(mono))
            {
                SelectButton(btns[0]);
                return false;
            }
            if (btns.Count == 1 && btns.Contains(mono))
            {
                //只有一个
                return false;
            }
            return true;
        }


        public void OnBtnClick()
        {
            if (mono != null)
            {
                mono.onClick?.Invoke();
            }

        }
    }

}
