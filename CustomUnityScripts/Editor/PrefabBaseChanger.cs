using UnityEngine;
using UnityEditor;

public class PrefabBaseChanger : EditorWindow
{
    static EditorWindow window;
    static GameObject newChild;
    GameObject newParent;
    PrefabReplacingSettings replacementSettings = new() {
        changeRootNameToAssetName = false,
        logInfo = true,
        objectMatchMode = ObjectMatchMode.ByHierarchy,
        prefabOverridesOptions = PrefabOverridesOptions.KeepAllPossibleOverrides
    };

    [MenuItem("Tools/Change Base Prefab of Prefab Instance", false, 151)]
    public static void Init() {
        window = GetWindow<PrefabBaseChanger>(true, "Change Base Prefab of Prefab Instance", true);
        window.minSize = new Vector2(400f, 200f);
        window.Show();
        newChild = Selection.activeGameObject;
    }

    public void OnGUI() {
        EditorGUIUtility.labelWidth = 200f;

        newChild = (GameObject) EditorGUILayout.ObjectField("Prefab instance to modify", newChild, typeof(GameObject), true);
        newParent = (GameObject) EditorGUILayout.ObjectField("Prefab asset to be the parent", newParent, typeof(GameObject), true);

        EditorGUILayout.Space();
        EditorGUIUtility.labelWidth = 150f;

        replacementSettings.changeRootNameToAssetName = GUILayout.Toggle(replacementSettings.changeRootNameToAssetName, "Change prefab instance's name to name of prefab asset");

        EditorGUILayout.Space();

        replacementSettings.objectMatchMode = (ObjectMatchMode) EditorGUILayout.EnumPopup("Object matching mode", replacementSettings.objectMatchMode);
        replacementSettings.prefabOverridesOptions = (PrefabOverridesOptions) EditorGUILayout.EnumFlagsField("Override options", replacementSettings.prefabOverridesOptions);
        EditorGUILayout.Space();

        if (GUILayout.Button("Rebase")) {
            PrefabUtility.ReplacePrefabAssetOfPrefabInstance(
                newChild,
                newParent,
                replacementSettings,
                InteractionMode.UserAction
            );
        }
    }
}
