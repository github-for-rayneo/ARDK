using RayNeo.Tool;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoubleTabBackSceneCtrl : MonoBehaviour
{
    public void OnDoubleTapCallBack()
    {
        SceneManager.LoadScene("SampleMain");
    }

    void Start()
    {
        TouchEventCtrl.Instance.OnDoubleTap += OnDoubleTapCallBack;
    }

    private void OnDestroy()
    {
        TouchEventCtrl.Instance.OnDoubleTap -= OnDoubleTapCallBack;

    }
}
