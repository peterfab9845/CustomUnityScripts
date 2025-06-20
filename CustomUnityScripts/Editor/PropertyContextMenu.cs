using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class PropertyContextMenu
{
    static PropertyContextMenu()
    {
        EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
    }

    public static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property) {
        // Save the current iterator position because it can change as the rest of the properties get rendered
        // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/EditorApplication-contextualPropertyMenu.html
        var propertyCopy = property.Copy();
        menu.AddItem(new GUIContent("Print info"), false, () =>
        {
            Debug.Log($"{propertyCopy.propertyPath}: (propertyType {propertyCopy.propertyType})(type {propertyCopy.type})");
        });
    }
}
