using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PropertyEditorWindow : EditorWindow
{
    public static void DrawSerializedObject(SerializedObject so, bool skipStart) {
        var props = so.GetIterator();
        if (skipStart) {
            props.NextVisible(enterChildren: true);
        } else {
            props.Next(enterChildren: true);
        }
        do {
            EditorGUILayout.PropertyField(props, includeChildren: true);
        } while (props.Next(enterChildren: false));
    }

    public void OnEnable()
    {
        EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
    }
    public void OnDestroy()
    {
        EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
    }
    public void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property) {
        // Save the current iterator position because it can change as the rest of the properties get rendered
        // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/EditorApplication-contextualPropertyMenu.html
        var propertyCopy = property.Copy();
        menu.AddItem(new GUIContent("Print info"), false, () =>
        {
            Debug.Log($"{propertyCopy.propertyPath}: (propertyType {propertyCopy.propertyType})(type {propertyCopy.type})");
        });
    }
} 
