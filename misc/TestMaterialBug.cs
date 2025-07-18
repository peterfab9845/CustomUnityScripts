using UnityEngine;
using UnityEditor;

public class TestMaterialBug {

    // setup:
    // matA = blank material
    // matB = variant of matA, also blank
    // matC = variant of matB, tex set
    // matD = variant of matB, tex set

    // step 1: flatten the material variants, they keep the tex. Then unload unused assets
    [MenuItem("Tools/Test Material Bug part1", false, 100)]
    private static void DoTestMaterialBug1()
    {
        Material matC = (Material) AssetDatabase.LoadAssetAtPath("Assets/test/matC.mat", typeof(Material));
        Material matD = (Material) AssetDatabase.LoadAssetAtPath("Assets/test/matD.mat", typeof(Material));
        matC.parent = null;
        matD.parent = null;
        EditorUtility.UnloadUnusedAssetsImmediate();
    }

    // step 2: reparent matC. It loses all its properties.
    [MenuItem("Tools/Test Material Bug part2", false, 101)]
    private static void DoTestMaterialBug2()
    {
        Material matB = (Material) AssetDatabase.LoadAssetAtPath("Assets/test/matB.mat", typeof(Material));
        Debug.Log(matB.parent);
        Material matC = (Material) AssetDatabase.LoadAssetAtPath("Assets/test/matC.mat", typeof(Material));
        matC.parent = matB;
    }

    // step 3: reparent matD. It does NOT lose all its properties (and neither do any following ones).
    [MenuItem("Tools/Test Material Bug part3", false, 102)]
    private static void DoTestMaterialBug3()
    {
        Material matB = (Material) AssetDatabase.LoadAssetAtPath("Assets/test/matB.mat", typeof(Material));
        Debug.Log(matB.parent);
        Material matD = (Material) AssetDatabase.LoadAssetAtPath("Assets/test/matD.mat", typeof(Material));
        matD.parent = matB;
    }
}
