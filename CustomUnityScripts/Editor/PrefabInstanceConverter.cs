using UnityEngine;
using UnityEditor;

public class PrefabInstanceConverter : EditorWindow
{
    static EditorWindow window;
    static GameObject newChild;
    GameObject newParent;
    ConvertToPrefabInstanceSettings conversionSettings = new() {
        changeRootNameToAssetName = false,
        componentsNotMatchedBecomesOverride = true,
        gameObjectsNotMatchedBecomesOverride = true,
        logInfo = true,
        objectMatchMode = ObjectMatchMode.ByHierarchy,
        recordPropertyOverridesOfMatches = true
    };

    [MenuItem("Tools/Convert GameObject to Prefab Instance", false, 150)]
    public static void Init() {
        window = GetWindow<PrefabInstanceConverter>(true, "Convert GameObject to Prefab Instance", true);
        window.minSize = new Vector2(400f, 200f);
        window.Show();
        newChild = Selection.activeGameObject;
    }

    public void OnGUI() {
        EditorGUIUtility.labelWidth = 250f;

        newChild = (GameObject) EditorGUILayout.ObjectField("GameObject to convert to prefab instance", newChild, typeof(GameObject), true);
        newParent = (GameObject) EditorGUILayout.ObjectField("Prefab asset to be the parent", newParent, typeof(GameObject), true);

        EditorGUILayout.Space();
        EditorGUIUtility.labelWidth = 150f;

        conversionSettings.changeRootNameToAssetName = GUILayout.Toggle(conversionSettings.changeRootNameToAssetName, "Change GameObject's name to name of prefab asset");

        EditorGUILayout.Space();

        conversionSettings.objectMatchMode = (ObjectMatchMode) EditorGUILayout.EnumPopup("Object matching mode", conversionSettings.objectMatchMode);
        conversionSettings.componentsNotMatchedBecomesOverride = GUILayout.Toggle(conversionSettings.componentsNotMatchedBecomesOverride, "Components not in prefab become overrides");
        conversionSettings.gameObjectsNotMatchedBecomesOverride = GUILayout.Toggle(conversionSettings.gameObjectsNotMatchedBecomesOverride, "GameObjects not matched become overrides");
        conversionSettings.recordPropertyOverridesOfMatches = GUILayout.Toggle(conversionSettings.recordPropertyOverridesOfMatches, "Values of matched properties become overrides");

        EditorGUILayout.Space();

        if (GUILayout.Button("Convert")) {
            PrefabUtility.ConvertToPrefabInstance(
                newChild,
                newParent,
                conversionSettings,
                InteractionMode.UserAction
            );
        }
    }
}
