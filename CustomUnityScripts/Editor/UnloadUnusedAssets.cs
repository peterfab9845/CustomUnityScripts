using UnityEditor;

public class UnloadUnusedAssets {

    [MenuItem("Tools/Unload Unused Assets", false, 170)]
    private static void DoUnloadUnusedAssets()
    {
        // https://docs.unity3d.com/2022.3/Documentation/ScriptReference/EditorUtility.UnloadUnusedAssetsImmediate.html
        // Unloads assets that are not used.
        // An asset is deemed to be unused if it isn't reached after walking the whole game object hierarchy, including script components. Static variables are also examined.
        // This method differs from Resources.UnloadUnusedAssets in that it will wait for asset garbage collection to finish before returning.
        EditorUtility.UnloadUnusedAssetsImmediate();
    }
}
