using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace RayNeo.Editor
{
    [InitializeOnLoad]
    public class EnvFix : EditorWindow
    {
        #region  Config

        private const string m_IgnorePrefix = "EnvFix";
        private static FixItem[] m_FixItems;
        private static EnvFix m_Window;
        private Vector2 m_ScrollPosition;
        private static AndroidSdkVersions m_MinSdkVersion = AndroidSdkVersions.AndroidApiLevel30;
        private static AndroidSdkVersions m_TargetSdkVersion = AndroidSdkVersions.AndroidApiLevel30;

        #endregion
        static EnvFix()
        {
            //ReimportDll();
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
            //EditorApplication.update -= ReimportDll;
            //EditorApplication.update += ReimportDll;
        }
        private static void ReimportDll()
        {
            if (EditorApplication.isUpdating)
            {
                return;
            }
            string tempLogPath = Application.temporaryCachePath + "/sdkVersion.txt";

            string _scriptName = "EnvFix";
            string[] path = UnityEditor.AssetDatabase.FindAssets(_scriptName);
            string envFixPath = AssetDatabase.GUIDToAssetPath(path[0]);
            string packagePath = envFixPath.Replace("Runtime/Editor/EnvFix.cs", "");
            string dllPath = packagePath + "Runtime/Plugin/UnityXRSDKCore.dll";
            string versionPath = packagePath + "package.json";
            string lineVersion = null;
            if (File.Exists(versionPath))
            {
                using (StreamReader sr = new StreamReader(versionPath))
                {
                    string line;
                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("version"))
                        {
                            //到行了.
                            lineVersion = line;
                            break;
                        }
                    }
                }
            }
            if (lineVersion == null)
            {
                return;
            }

            using (FileStream fs = new FileStream(tempLogPath, FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                string line = Encoding.UTF8.GetString(buffer);
                if (lineVersion.Equals(line))
                {
                    //版本一致.不处理
                    return;
                }
                buffer = Encoding.UTF8.GetBytes(lineVersion);
                fs.Seek(0, SeekOrigin.Begin);
                fs.SetLength(0);
                fs.Write(buffer, 0, buffer.Length);
            }

            Debug.Log("检测没有reimport.  重新导入:" + dllPath + " File:" + File.Exists(dllPath));
            AssetDatabase.ImportAsset(dllPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.DontDownloadFromCacheServer);
            EditorApplication.update -= ReimportDll;

        }

        private static void OnUpdate()
        {

            bool show = false;

            if (m_FixItems == null) { RegisterItems(); }
            foreach (var item in m_FixItems)
            {
                if (!item.IsIgnored() && !item.IsValid() && item.Level > MessageType.Warning && item.IsAutoPop())
                {
                    show = true;
                }
            }

            if (show)
            {
                ShowWindow();
            }

            EditorApplication.update -= OnUpdate;
        }

        private static void RegisterItems()
        {
            m_FixItems = new FixItem[]
            {
            //new CkeckMTRendering(MessageType.Error),
            new CkeckAndroidGraphicsAPI(MessageType.Error),
            new CkeckMTRendering(MessageType.Error),
            new CkeckAndroidOrientation(MessageType.Warning),
            new CkeckColorSpace(MessageType.Warning),
            new CheckOptimizedFramePacing(MessageType.Warning),
            new CheckMinmumAPILevel(MessageType.Error),
            new CheckTargetAPILevel(MessageType.Error),
            new CheckScriptingBackendAndTargetArc(MessageType.Error)
            };
        }

        [MenuItem("RayNeo/Env/Project Environment Fix", false)]
        public static void ShowWindow()
        {
            RegisterItems();
            m_Window = GetWindow<EnvFix>(true);
            m_Window.minSize = new Vector2(320, 300);
            m_Window.maxSize = new Vector2(320, 800);
            m_Window.titleContent = new GUIContent("XRSDK | Environment Fix");
            ReimportDll();
        }

        //[MenuItem("UXR/Env/Delete Ignore", false)]
        public static void DeleteIgonre()
        {
            foreach (var item in m_FixItems)
            {
                item.DeleteIgonre();
            }
        }

        public void OnGUI()
        {
            string resourcePath = GetResourcePath();
            Texture2D logo = AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath + "LOGO.png");
            Rect rect = GUILayoutUtility.GetRect(position.width, 80, GUI.skin.box);
            GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);

            string aboutText = "This window provides tips to help fix common issues with the XRSDK and your project.";
            EditorGUILayout.LabelField(aboutText, EditorStyles.textArea);

            int ignoredCount = 0;
            int fixableCount = 0;
            int invalidNotIgnored = 0;

            for (int i = 0; i < m_FixItems.Length; i++)
            {
                FixItem item = m_FixItems[i];

                bool ignored = item.IsIgnored();
                bool valid = item.IsValid();
                bool fixable = item.IsFixable();

                if (!valid && !ignored && fixable)
                {
                    fixableCount++;
                }

                if (!valid && !ignored)
                {
                    invalidNotIgnored++;
                }

                if (ignored)
                {
                    ignoredCount++;
                }
            }

            Rect issuesRect = EditorGUILayout.GetControlRect();
            GUI.Box(new Rect(issuesRect.x - 4, issuesRect.y, issuesRect.width + 8, issuesRect.height), "Tips", EditorStyles.toolbarButton);

            if (invalidNotIgnored > 0)
            {
                m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
                {
                    for (int i = 0; i < m_FixItems.Length; i++)
                    {
                        FixItem item = m_FixItems[i];

                        if (!item.IsIgnored() && !item.IsValid())
                        {
                            invalidNotIgnored++;

                            GUILayout.BeginVertical("box");
                            {
                                item.DrawGUI();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    // Aligns buttons to the right
                                    GUILayout.FlexibleSpace();

                                    if (item.IsFixable())
                                    {
                                        if (GUILayout.Button("Fix"))
                                            item.Fix();
                                    }

                                    //if (GUILayout.Button("Ignore"))
                                    //    check.Ignore();
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                        }
                    }
                }
                GUILayout.EndScrollView();
            }

            GUILayout.FlexibleSpace();

            if (invalidNotIgnored == 0)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label("No issues found");

                        if (GUILayout.Button("Close Window"))
                            Close();
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.BeginHorizontal("box");
            {
                if (fixableCount > 0)
                {
                    if (GUILayout.Button("Accept All"))
                    {
                        if (EditorUtility.DisplayDialog("Accept All", "Are you sure?", "Yes, Accept All", "Cancel"))
                        {
                            for (int i = 0; i < m_FixItems.Length; i++)
                            {
                                FixItem item = m_FixItems[i];

                                if (!item.IsIgnored() &&
                                    !item.IsValid())
                                {
                                    if (item.IsFixable())
                                        item.Fix();
                                }
                            }
                        }
                    }
                }

            }
            GUILayout.EndHorizontal();
        }

        private string GetResourcePath()
        {
            var ms = MonoScript.FromScriptableObject(this);
            var path = AssetDatabase.GetAssetPath(ms);
            path = Path.GetDirectoryName(path);
            return path + "\\Textures\\";
        }

        private abstract class FixItem
        {
            protected string key;
            protected MessageType level;

            public MessageType Level
            {
                get
                {
                    return level;
                }
            }

            public FixItem(MessageType level)
            {
                this.level = level;
            }

            public void Ignore()
            {
                EditorPrefs.SetBool(m_IgnorePrefix + key, true);
            }

            public bool IsIgnored()
            {
                return EditorPrefs.HasKey(m_IgnorePrefix + key);
            }

            public void DeleteIgonre()
            {
                Debug.Log("DeleteIgnore" + m_IgnorePrefix + key);
                EditorPrefs.DeleteKey(m_IgnorePrefix + key);
            }

            public abstract bool IsValid();

            public abstract bool IsAutoPop();

            public abstract void DrawGUI();

            public abstract bool IsFixable();

            public abstract void Fix();

            protected void DrawContent(string title, string msg)
            {
                EditorGUILayout.HelpBox(title, level);
                EditorGUILayout.LabelField(msg, EditorStyles.textArea);
            }


            protected bool CheckAndroidPlantform()
            {
                if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                {
                    if (EditorUtility.DisplayDialog("安卓平台切换", "当前您不是安卓平台,是否切换?", "切换到Android平台", "暂不"))
                    {
                        if (EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
                        {
                            return true;
                        }
                    }
                    //EditorGUILayout.HelpBox("当前您不是安卓平台,是否切换?",MessageType.Warning);
                }
                else
                {
                    return true;
                }

                return false;
            }

        }

        private class CkeckAndroidGraphicsAPI : FixItem
        {
            public CkeckAndroidGraphicsAPI(MessageType level) : base(level)
            {
                key = this.GetType().Name;
            }

            public override bool IsValid()
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    if (PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android))
                    {
                        return false;
                    }
                    var graphics = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                    if (graphics != null && graphics.Length >= 1 && graphics[0] == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }

            public override void DrawGUI()
            {
                string message = @"In order to render correct on mobile devices, the graphicsAPIs should be set to OpenGLES. 
in dropdown list of Player Settings > Other Settings > Graphics APIs , choose 'OpenGLES2 or OpenGLES3'.";
                DrawContent("GraphicsAPIs is not OpenGLES", message);
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override void Fix()
            {
                if (CheckAndroidPlantform())
                {
                    PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new GraphicsDeviceType[1] { GraphicsDeviceType.OpenGLES3 });
                }
            }

            public override bool IsAutoPop()
            {
                return true;
            }
        }

        private class CkeckMTRendering : FixItem
        {
            public CkeckMTRendering(MessageType level) : base(level)
            {
                key = this.GetType().Name;
            }

            public override bool IsValid()
            {
                return !PlayerSettings.GetMobileMTRendering(BuildTargetGroup.Android);
            }

            public override void DrawGUI()
            {

                string message = @"In order to run correct on mobile devices, the RenderingThreadingMode should be set. 
in dropdown list of Player Settings > Other Settings > Multithreaded Rendering, close toggle.";
                DrawContent("Multithreaded Rendering not close", message);
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override void Fix()
            {
                if (CheckAndroidPlantform())
                {
                    PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, false);
                }
            }

            public override bool IsAutoPop()
            {
                return true;
            }
        }

        private class CkeckAndroidOrientation : FixItem
        {
            public CkeckAndroidOrientation(MessageType level) : base(level)
            {
                key = this.GetType().Name;
            }

            public override bool IsValid()
            {
                return PlayerSettings.defaultInterfaceOrientation == UIOrientation.LandscapeLeft;
            }

            public override void DrawGUI()
            {
                string message = @"In order to display correct on mobile devices, the orientation should be set to LandscapeLeft. 
in dropdown list of Player Settings > Resolution and Presentation > Default Orientation, choose 'LandscapeLeft'.";
                DrawContent("Orientation is not LandscapeLeft", message);
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override void Fix()
            {
                if (CheckAndroidPlantform())
                {
                    PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
                }
            }

            public override bool IsAutoPop()
            {
                return true;
            }
        }

        private class CkeckColorSpace : FixItem
        {
            public CkeckColorSpace(MessageType level) : base(level)
            {
                key = this.GetType().Name;
            }

            public override bool IsValid()
            {
                return PlayerSettings.colorSpace == ColorSpace.Gamma;
            }

            public override void DrawGUI()
            {
                string message = @"In order to display correct on mobile devices, the colorSpace should be set to gamma. 
in dropdown list of Player Settings > Other Settings > Color Space, choose 'Gamma'.";
                DrawContent("ColorSpace is not Linear", message);
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override void Fix()
            {
                if (CheckAndroidPlantform())
                {
                    PlayerSettings.colorSpace = ColorSpace.Gamma;
                }
            }

            public override bool IsAutoPop()
            {
                return true;
            }
        }

        private class CkeckAndroidPermission : FixItem
        {
            public CkeckAndroidPermission(MessageType level) : base(level)
            {
                key = this.GetType().Name;
            }

            public override bool IsValid()
            {
                return PlayerSettings.Android.forceInternetPermission;
            }

            public override void DrawGUI()
            {
                string message = @"In order to run correct on mobile devices, the internet access premission should be set. 
in dropdown list of Player Settings > Other Settings > Internet Access, choose 'Require'.";
                DrawContent("internet access permission not available", message);
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override void Fix()
            {
                if (CheckAndroidPlantform())
                {
                    PlayerSettings.Android.forceInternetPermission = true;
                }
            }

            public override bool IsAutoPop()
            {
                return true;
            }
        }

        private class CheckOptimizedFramePacing : FixItem
        {
            public CheckOptimizedFramePacing(MessageType level) : base(level)
            {
                key = this.GetType().Name;
            }

            public override void DrawGUI()
            {
                string message = @"The optimizedFramePacing need to unselect";
                DrawContent("OptimizedFramePacing", message);
            }

            public override void Fix()
            {
                PlayerSettings.Android.optimizedFramePacing = false;
            }

            public override bool IsAutoPop()
            {
                return true;
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override bool IsValid()
            {
                return PlayerSettings.Android.optimizedFramePacing == false;
            }
        }

        private class CheckMinmumAPILevel : FixItem
        {
            public CheckMinmumAPILevel(MessageType level) : base(level)
            {
                key = this.GetType().Name;
            }

            public override void DrawGUI()
            {
                string message = @"The minSdkVersion need to select " + m_MinSdkVersion.ToString();
                DrawContent("MinSDKVersion", message);
            }

            public override void Fix()
            {
                if (CheckAndroidPlantform())
                {
                    PlayerSettings.Android.minSdkVersion = m_MinSdkVersion;
                }
            }

            public override bool IsAutoPop()
            {
                return true;
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override bool IsValid()
            {
                return PlayerSettings.Android.minSdkVersion >= m_MinSdkVersion;
            }
        }

        private class CheckTargetAPILevel : FixItem
        {
            public CheckTargetAPILevel(MessageType level) : base(level)
            {
                key = this.GetType().Name;
            }

            public override void DrawGUI()
            {
                string message = @"The targetSdkVersion need to select " + m_TargetSdkVersion.ToString();
                DrawContent("TargetSDKVersion", message);
            }

            public override void Fix()
            {
                if (CheckAndroidPlantform())
                {
                    PlayerSettings.Android.targetSdkVersion = m_TargetSdkVersion;
                }
            }

            public override bool IsAutoPop()
            {
                return true;
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override bool IsValid()
            {
                return PlayerSettings.Android.targetSdkVersion >= m_TargetSdkVersion;
            }
        }

        private class CheckScriptingBackendAndTargetArc : FixItem
        {
            public CheckScriptingBackendAndTargetArc(MessageType level) : base(level)
            {
                key = this.GetType().Name;
            }

            public override void DrawGUI()
            {
                string message = @"The scripting backend should be set to IL2CPP and the target architectures should be set to ARM64.
in dropdown list of Player Settings > Other Settings > Scripting Backend and Target Architectures.";
                DrawContent("ScriptingBackend And TargetArchitectures", message);
            }

            public override void Fix()
            {
                if (CheckAndroidPlantform())
                {
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
                }
            }

            public override bool IsAutoPop()
            {
                return true;
            }

            public override bool IsFixable()
            {
                return true;
            }

            public override bool IsValid()
            {
                return (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) == ScriptingImplementation.IL2CPP) &&
                (PlayerSettings.Android.targetArchitectures == AndroidArchitecture.ARM64);
            }
        }
    }
}