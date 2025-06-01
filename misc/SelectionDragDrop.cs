// Put this script in Assets/Editor/ so it doesn't get built with the project!

using UnityEngine;
using UnityEditor;

internal class SelectionDragDrop : EditorWindow {

    [MenuItem("Tools/Print Selection Type", false, 50)]
    public static void PrintSelectionType(MenuCommand menuCommand)
    {
        Debug.Log(Selection.activeObject.GetType().Name);
    }

    [MenuItem("Tools/Print Selection Type2", false, 50)]
    static void Init() => GetWindow(typeof(SelectionDragDrop)).Show();
    
    void OnSelectionChange()
    {
        Debug.Log(Selection.objects[0].GetType().Name);
    }

}
