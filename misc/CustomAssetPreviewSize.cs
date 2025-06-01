using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class CustomAssetPreviewSize : EditorWindow
{
    static readonly Type _projectBrowserType;
    static readonly FieldInfo _projectBrowserListAreaField;
    static readonly PropertyInfo _listAreaGridSizeField;

    static CustomAssetPreviewSize()
    {
        try
        {
            _projectBrowserType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
            _projectBrowserListAreaField = _projectBrowserType.GetField("m_ListArea", BindingFlags.NonPublic | BindingFlags.Instance);

            var listAreaType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ObjectListArea");
            _listAreaGridSizeField = listAreaType.GetProperty("gridSize", BindingFlags.Public | BindingFlags.Instance);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    [MenuItem("Tools/Make asset previews big", false, 500)]
    public static void MakeBig()
    {
        var editorWindow = GetWindow(_projectBrowserType, false, null, false);
        var listArea = _projectBrowserListAreaField.GetValue(editorWindow);
        if (listArea != null)
        {
            _listAreaGridSizeField.SetValue(listArea, 512);
            EditorApplication.RepaintProjectWindow();
        }
    }

}
