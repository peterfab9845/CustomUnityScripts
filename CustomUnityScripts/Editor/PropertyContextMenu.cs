using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class PropertyContextMenu
{
    static PropertyContextMenu()
    {
        EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
    }

    public static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty originalProperty) {
        // Save the current iterator position because it can change as the rest of the properties get rendered
        // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/EditorApplication-contextualPropertyMenu.html
        var property = originalProperty.Copy();
        menu.AddItem(new GUIContent("Print property info"), false, () =>
        {
            Debug.Log($"{property.propertyPath}: (propertyType {property.propertyType})(type {property.type})");
        });
    }
}
