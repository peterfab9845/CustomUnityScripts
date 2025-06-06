using UnityEditor;
using UnityEngine;

public class ObjectPropertyEditor : PropertyEditorWindow
{
    static EditorWindow window;
    Vector2 scrollPos;
    static Object obj;
    static bool refreshObj;
    SerializedObject so;

    [MenuItem("Tools/Object Property Editor", false, 120)]
    private static void Init() {
        window = GetWindow<ObjectPropertyEditor>("Object Property Editor", true);

        Object sel = Selection.activeObject;
        if (sel != null) {
            obj = sel;
            refreshObj = true;
        }
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        obj = EditorGUILayout.ObjectField("Object", obj, typeof(Object), true);
        refreshObj |= EditorGUI.EndChangeCheck();
        refreshObj |= GUILayout.Button("Read serialized properties");

        if (refreshObj) {
            so = (obj == null) ? null : new SerializedObject(obj);
            refreshObj = false;
        }

        EditorGUI.BeginDisabledGroup(so == null);
        if (GUILayout.Button("Write serialized properties")) {
            so.ApplyModifiedProperties();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (so != null) {
            DrawSerializedObject(so, skipStart: false, showHidden: true);
        }
        EditorGUILayout.EndScrollView();
    }
}
