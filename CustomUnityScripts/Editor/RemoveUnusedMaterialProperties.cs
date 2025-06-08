//
// RemoveUnusedMaterialProperties.cs
// http://light11.hatenadiary.com/entry/2018/12/04/224253
//
using UnityEngine;
using UnityEditor;

public class RemoveUnusedMaterialProperties : EditorWindow {

    [MenuItem("CONTEXT/Material/Remove Unused Properties")]
    private static void RemoveUnusedProperties(MenuCommand menuCommand)
    {
        Material mat = (Material) menuCommand.context;
        var so = new SerializedObject(mat);
        var savedProp = so.FindProperty("m_SavedProperties");

        // Tex Envs
        var texProp = savedProp.FindPropertyRelative("m_TexEnvs");
        for (int i = texProp.arraySize - 1; i >= 0; i--) {
            var propertyName = texProp.GetArrayElementAtIndex(i).FindPropertyRelative("first").stringValue;
            if (!mat.HasTexture(propertyName)) {
                Debug.Log($"Removing texture property {propertyName}", mat);
                texProp.DeleteArrayElementAtIndex(i);
            }
        }

        // Integers
        var intProp = savedProp.FindPropertyRelative("m_Ints");
        for (int i = intProp.arraySize - 1; i >= 0; i--) {
            var propertyName = intProp.GetArrayElementAtIndex(i).FindPropertyRelative("first").stringValue;
            if (!mat.HasInteger(propertyName)) {
                Debug.Log($"Removing integer property {propertyName}", mat);
                intProp.DeleteArrayElementAtIndex(i);
            }
        }

        // Floats
        var floatProp = savedProp.FindPropertyRelative("m_Floats");
        for (int i = floatProp.arraySize - 1; i >= 0; i--) {
            var propertyName = floatProp.GetArrayElementAtIndex(i).FindPropertyRelative("first").stringValue;
            if (!mat.HasFloat(propertyName)) {
                Debug.Log($"Removing float property {propertyName}", mat);
                floatProp.DeleteArrayElementAtIndex(i);
            }
        }

        // Colors
        var colorProp = savedProp.FindPropertyRelative("m_Colors");
        for (int i = colorProp.arraySize - 1; i >= 0; i--) {
            var propertyName = colorProp.GetArrayElementAtIndex(i).FindPropertyRelative("first").stringValue;
            if (!mat.HasColor(propertyName)) {
                Debug.Log($"Removing color property {propertyName}", mat);
                colorProp.DeleteArrayElementAtIndex(i);
            }
        }

        so.ApplyModifiedProperties();
    }
}
