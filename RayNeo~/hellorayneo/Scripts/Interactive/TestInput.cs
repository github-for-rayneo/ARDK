using RayNeo.Tool;
using UnityEngine;
using UnityEngine.UI;
public class TestInput : MonoBehaviour
{
    [SerializeField]
    private Text InputTxt;

    private void OnEnable()
    {
        TouchEventCtrl.Instance.OnTouchStart += OnTouchStart;
        TouchEventCtrl.Instance.OnTouchDown  += OnTouchDown;
        TouchEventCtrl.Instance.OnTouchUp    += OnTouchUp;

        TouchEventCtrl.Instance.OnSwipeLeftEnd  += OnSwipeLeftEnd;
        TouchEventCtrl.Instance.OnSwipeRightEnd += OnSwipeRightEnd;

        TouchEventCtrl.Instance.OnSwipeLeft  += OnSwipeLeft;
        TouchEventCtrl.Instance.OnSwipeRight += OnSwipeRight;

        TouchEventCtrl.Instance.OnSimpleTap += OnSimpleTap;
        TouchEventCtrl.Instance.OnDoubleTap += OnDoubleTap;
        TouchEventCtrl.Instance.OnTripleTap += OnTripleTap;

        TouchEventCtrl.Instance.OnLongTap += OnLongTap;
    }

    private void OnDisable()
    {
        TouchEventCtrl.Instance.OnTouchStart -= OnTouchStart;
        TouchEventCtrl.Instance.OnTouchDown  -= OnTouchDown;
        TouchEventCtrl.Instance.OnTouchUp    -= OnTouchUp;

        TouchEventCtrl.Instance.OnSwipeLeftEnd  -= OnSwipeLeftEnd;
        TouchEventCtrl.Instance.OnSwipeRightEnd -= OnSwipeRightEnd;

        TouchEventCtrl.Instance.OnSwipeLeft  -= OnSwipeLeft;
        TouchEventCtrl.Instance.OnSwipeRight -= OnSwipeRight;


        TouchEventCtrl.Instance.OnDoubleTap -= OnDoubleTap;
        TouchEventCtrl.Instance.OnSimpleTap -= OnSimpleTap;
        TouchEventCtrl.Instance.OnTripleTap -= OnTripleTap;

        TouchEventCtrl.Instance.OnLongTap   -= OnLongTap;
    }


    private void OnTouchStart()
    {
        Debug.Log("[RayNeoX2]:OnTouchStart");
        InputTxt.text = "OnTouchStart";
    }

    private void OnTouchDown()
    {
        Debug.Log("[RayNeoX2]:OnTouchDown");
        InputTxt.text = "OnTouchDown";
    }

    private void OnTouchUp()
    {
        Debug.Log("[RayNeoX2]:OnTouchUp");
        InputTxt.text = "OnTouchUp";
    }

    private void OnSwipeLeftEnd()
    {
        Debug.Log("[RayNeoX2]:OnSwipeLeftEnd");
        InputTxt.text = "OnSwipeLeftEnd";
    }

    private void OnSwipeRightEnd()
    {
        Debug.Log("[RayNeoX2]:OnSwipeRightEnd");
        InputTxt.text = "OnSwipeRightEnd";
    }

    private void OnSwipeLeft()
    {
        Debug.Log("[RayNeoX2]:OnSwipeLeft");
        InputTxt.text = "OnSwipeLeft";
    }

    private void OnSwipeRight()
    {
        Debug.Log("[RayNeoX2]:OnSwipeRight");
        InputTxt.text = "OnSwipeRight";
    }

    private void OnDoubleTap()
    {
        Debug.LogError("[RayNeoX2]:OnDoubleTap");
        InputTxt.text = "OnDoubleTap";
    }

    private void OnSimpleTap()
    {
        Debug.LogError("[RayNeoX2]:OnSimpleTap");
        InputTxt.text = "OnSimpleTap";
    }
    private void OnTripleTap()
    {
        Debug.LogError("[RayNeoX2]:OnTripleTap");
        InputTxt.text = "OnTripleTap";
    }

    private void OnLongTap()
    {
        Debug.LogError("[RayNeoX2]:OnLongTap");
        InputTxt.text = "OnLongTap";
    }
}
