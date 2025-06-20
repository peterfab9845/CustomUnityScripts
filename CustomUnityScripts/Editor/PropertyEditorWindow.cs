using UnityEditor;

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
}
