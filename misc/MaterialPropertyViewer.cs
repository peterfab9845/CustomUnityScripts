using System;
using UnityEditor;
using UnityEngine;

// This one shows the _runtime_ properties, and only the ones which exist on the material's current shader

public class MaterialPropertyViewer : EditorWindow
{
    static EditorWindow window;
    Vector2 scrollPos;
    static Material mat;

    [MenuItem("Tools/Material Property Viewer", false, 130)]
    private static void Init() {
        window = GetWindow<MaterialPropertyViewer>(true, "Material Property Viewer", true);
        window.Show();
        mat = Selection.activeObject as Material;
    }

    private void OnGUI()
    {
        mat = (Material) EditorGUILayout.ObjectField("Material", mat, typeof(Material), true);

        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (mat != null) {
            foreach (MaterialPropertyType t in Enum.GetValues(typeof(MaterialPropertyType))) {
                string[] names = mat.GetPropertyNames(t);
                Array.Sort(names);
                foreach (string name in names) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(name);
                    EditorGUILayout.LabelField(t.ToString(), GUILayout.MinWidth(100));
                    EditorGUILayout.LabelField($"HasFloat: {mat.HasFloat(name)}", GUILayout.MinWidth(100));
                    EditorGUILayout.LabelField($"HasInteger: {mat.HasInteger(name)}", GUILayout.MinWidth(100));
                    EditorGUILayout.LabelField($"HasVector: {mat.HasVector(name)}", GUILayout.MinWidth(100));
                    EditorGUILayout.LabelField($"HasMatrix: {mat.HasMatrix(name)}", GUILayout.MinWidth(100));
                    EditorGUILayout.LabelField($"HasTexture: {mat.HasTexture(name)}", GUILayout.MinWidth(100));
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
