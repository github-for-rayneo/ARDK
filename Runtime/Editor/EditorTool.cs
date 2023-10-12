using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 部分Editor常用工具整理
/// </summary>
public class EditorTool
{
    static string[] AssetGUIDs;
    static string[] AssetPaths;
    static string[] AllAssetPaths;

    [MenuItem("Assets/资源引用查找", false, 25)]
    static void FindreAssetFerencesMenu()
    {
        Debug.LogError("开始查找资源引用");

        if (Selection.assetGUIDs.Length == 0)
        {
            Debug.LogError("请在Project文件夹中选择需要查找的资源文件，右键并选择“资源引用查找”");
            return;
        }

        AssetGUIDs = Selection.assetGUIDs;
        AssetPaths = new string[AssetGUIDs.Length];

        for (int i = 0; i < AssetGUIDs.Length; i++)
        {
            AssetPaths[i] = AssetDatabase.GUIDToAssetPath(AssetGUIDs[0]);
            Debug.LogError("[MercuryX2]:AssetPath|" + AssetPaths[i]);
        }

        AllAssetPaths = AssetDatabase.GetAllAssetPaths();
        FindreAssetReferences();
    }

    static void FindreAssetReferences()
    {
        List<string> logInfo = new List<string>();
        string path;
        string log;
        for (int i = 0; i < AllAssetPaths.Length; i++)
        {
            path = AllAssetPaths[i];

            if (path.EndsWith(".prefab") || path.EndsWith(".unity"))
            {
                string content = File.ReadAllText(path);
                if (content == null)
                {
                    continue;
                }
                for (int j = 0; j < AssetGUIDs.Length; j++)
                {
                    if (content.IndexOf(AssetGUIDs[j]) > 0)
                    {
                        log = string.Format("{0}引用{1}", path, AssetPaths[j]);
                        logInfo.Add(log);
                    }
                }
            }
        }

        for (int i = 0; i < logInfo.Count; i++)
        {
            Debug.LogError(logInfo[i]);
        }
        Debug.LogError("[MercuryX2]:选择对象引用数量:" + logInfo.Count);
    }
}