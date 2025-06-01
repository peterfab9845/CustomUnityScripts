using UnityEditor;
using UnityEngine;

public class MaterialPropertyTest : EditorWindow
{
    [MenuItem("CONTEXT/Material/set props", false, 100)]
    private static void SetMaterialProperties(MenuCommand menuCommand) {
        Material mat = (Material) menuCommand.context;
        Debug.Log("Setting props on " + mat.name);
        Debug.Log($"START: serialized: {mat.GetFloat("_AAAATestFloatSerialized")}, api: {mat.GetFloat("_AAAATestFloatMaterialAPI")}");

        // SERIALIZED METHOD
        // Gets stored and uploaded with the material, so it works.
        // Appears in debug inspector "saved properties".
        // Is NOT seen by Material.* functions.
        var so = new SerializedObject(mat);
        so.Update();
        var savedProps = so.FindProperty("m_SavedProperties");

        var floatProps = savedProps.FindPropertyRelative("m_Floats");

        floatProps.InsertArrayElementAtIndex(floatProps.arraySize);
        var newProp = floatProps.GetArrayElementAtIndex(floatProps.arraySize-1);
        newProp.FindPropertyRelative("first").stringValue = "_AAAATestFloatSerialized";
        newProp.FindPropertyRelative("second").floatValue = 123.0f;

        so.ApplyModifiedProperties();

        // MATERIAL API METHOD
        // Doesn't get serialized, so isn't saved or uploaded (so it's useless).
        // While it temporarily exists, it is seen by Material.* functions, but not in the serialized properties.
        mat.SetFloat("_AAAATestFloatMaterialAPI", 456.0f);

        Debug.Log($"END: serialized: {mat.GetFloat("_AAAATestFloatSerialized")}, api: {mat.GetFloat("_AAAATestFloatMaterialAPI")}");

        // Matrices (and I assume both types of buffers) are the same, I assume they're only used through scripts at runtime
        // mat.SetMatrix("_AAAATestMatrix", Matrix4x4.zero); // default is identity
        // Debug.Log($"MATRIX: {mat.GetMatrix("_AAAATestMatrix")}");
    }

    [MenuItem("CONTEXT/Material/check props", false, 100)]
    private static void CheckMaterialProperties(MenuCommand menuCommand) {
        Material mat = (Material) menuCommand.context;
        Debug.Log("Checking props on " + mat.name);
        Debug.Log($"CHECK: serialized: {mat.GetFloat("_AAAATestFloatSerialized")}, api: {mat.GetFloat("_AAAATestFloatMaterialAPI")}");
        // Debug.Log($"MATRIX: {mat.GetMatrix("_AAAATestMatrix")}");
    }
}
