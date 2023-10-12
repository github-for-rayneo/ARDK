using RayNeo.Native;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RayNeo.Editor
{
    public class DebugMono : MonoBehaviour
    {
        public static DebugMono dm;
        private bool onCtrl = false;//是否按下了ctrl  以用鼠标键盘控制

        private Vector3 oldGlassV3;//进入控制的偏移量.
        private Vector3 oldMobileV3;//进入控制的偏移量.
        private Vector3 mouseStartTag;//鼠标记录值

        float xRotOff = 0;//相机的偏移积累量
        float yRotOff = 0;//相机的偏移积累量
        float zRotOff = 0;//相机的偏移积累量

        float xPosOff = 0;
        float yPosOff = 0;
        float zPosOff = 0;

        private float mouseToRayRate = 54;//鼠标位移到射线旋转的比值
        private float cameraMoveSpeed = 0.5f;//相机移动的速率

        public static void AddMono()
        {

            if (dm != null)
            {
                return;
            }
            else
            {
                GameObject go = (GameObject)GameObject.FindObjectOfType(typeof(DebugMono));
                if (go != null)
                {
                    dm = go.GetComponent<DebugMono>();
                    if (dm != null)
                    {
                        return;
                    }
                }
            }
            new GameObject("DebugMono").AddComponent<DebugMono>();
        }
        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneChange;
            if (dm)
            {
                Destroy(this);
                return;
            }
            dm = this;
            DontDestroyOnLoad(this);
        }
        private void OnSceneChange(Scene s, LoadSceneMode m)
        {

        }

        private void onEnterCtrl()
        {
            Quaternion glassQ = NativeInfo.Instance.GetGlassesQualternion();
            if (glassQ == Quaternion.identity)
            {
                oldGlassV3 = Vector3.zero;
            }
            else
            {
                oldGlassV3 = glassQ.eulerAngles;
            }

            Quaternion mobileQ = NativeInfo.Instance.GetMobileQualternion();

            if (mobileQ == Quaternion.identity)
            {
                oldMobileV3 = Vector3.zero;
            }
            else
            {
                oldMobileV3 = mobileQ.eulerAngles;
            }
            mouseStartTag = Input.mousePosition;
            xRotOff = 0;
            yRotOff = 0;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                onCtrl = !onCtrl;
                if (onCtrl)
                {
                    //处理初始化
                    onEnterCtrl();
                }
            }

            if (!onCtrl)
            {
                return;
            }

            HandleRayRotate(NativeInfo.Instance.WindowsMessager);
            HandleCameraRotate(NativeInfo.Instance.WindowsMessager);
        }
        private void HandleRayRotate(WindowsMessager wmsger)
        {
            Vector3 offset = (mouseStartTag - Input.mousePosition) / mouseToRayRate;
            wmsger.m_windowsMouseQuaternion = Quaternion.Euler(oldMobileV3.x + offset.y, oldMobileV3.y - offset.x, oldMobileV3.z + offset.z);

        }
        private void HandleCameraRotate(WindowsMessager wmsger)
        {
            if (Input.GetKey(KeyCode.W))
            {
                xRotOff += -cameraMoveSpeed;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                xRotOff += cameraMoveSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                yRotOff += -cameraMoveSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                yRotOff += cameraMoveSpeed;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                zRotOff += -cameraMoveSpeed;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                zRotOff += cameraMoveSpeed;
            }
            FfalconXR.HeadTrackedPoseDriver.SetQuaternion(Quaternion.Euler(xRotOff, yRotOff, zRotOff));
            if (Input.GetKey(KeyCode.J))
            {
                xPosOff += -cameraMoveSpeed;
            }
            else if (Input.GetKey(KeyCode.L))
            {
                xPosOff += cameraMoveSpeed;
            }
            if (Input.GetKey(KeyCode.K))
            {
                zPosOff += -cameraMoveSpeed;
            }
            else if (Input.GetKey(KeyCode.I))
            {
                zPosOff += cameraMoveSpeed;
            }
            if (Input.GetKey(KeyCode.U))
            {
                yPosOff += -cameraMoveSpeed;
            }
            else if (Input.GetKey(KeyCode.O))
            {
                yPosOff += cameraMoveSpeed;
            }

            FfalconXR.HeadTrackedPoseDriver.SetPosition(new Vector3(xPosOff, yPosOff, zPosOff) * 3);


        }

        private void OnGUI()
        {
            if (onCtrl)
            {
                //这是水星的提示符
                GUILayout.Label("鼠标控制射线左右上下\nwasd控制镜头左右上下,qe控制旋转\nijkl控制平面位置，uo控制上下位置");
            }
        }

    }

    [InitializeOnLoad]
    public class DebugWindow
    {
        private static string cachedCurrentScene;
        private static string currentScene
        {
            get
            {
                return cachedCurrentScene;
            }
            set
            {
                cachedCurrentScene = value;
            }
        }
        static DebugWindow()
        {

            EditorApplication.update += Update;
        }

        public static void Update()
        {
            if (EditorApplication.isPlaying)
            {
                if (DebugMono.dm == null)
                {
                    DebugMono.AddMono();
                }
            }
        }
    }

}