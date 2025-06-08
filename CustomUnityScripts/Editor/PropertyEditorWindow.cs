using UnityEditor;
using UnityEngine;

public class PropertyEditorWindow : EditorWindow
{
    public static void DrawSerializedObject(SerializedObject so, bool skipStart, bool showHidden) {
        var prop = so.GetIterator();
        if (skipStart) {
            // go past Unity default fields
            prop.NextVisible(enterChildren: true);
        } else {
            prop.Next(enterChildren: true);
        }

        bool savedForceChildVisibility = so.forceChildVisibility;
        if (showHidden) {
            so.forceChildVisibility = true;
        }

        do {
            EditorGUILayout.PropertyField(prop, includeChildren: true); // always uses NextVisible
        } while (prop.NextVisible(enterChildren: false));

        so.forceChildVisibility = savedForceChildVisibility;
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
