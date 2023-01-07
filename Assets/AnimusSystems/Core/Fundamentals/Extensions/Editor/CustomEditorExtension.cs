using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CustomEditorExtension
{
    public static string GetProjectWindowPath()
    {
        if (Selection.activeObject == null) return "Assets/";
        var path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
        if (path.Length == 0) return "Assets/";
        if (Directory.Exists(path)) return path + "/";
        int LastDashIndex = path.Length - 1;
        while (path[LastDashIndex] != '/') LastDashIndex--;
        return path.Substring(0, LastDashIndex) + "/";
    }
}
