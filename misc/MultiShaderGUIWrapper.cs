// Put this script in Assets/Editor/ so it doesn't get built with the project!

using UnityEngine;
using UnityEditor;
using VRC.ToonStandard;
using lilToon;

internal class MultiShaderGUIWrapper : ShaderGUI {

    lilToonInspector lilToonEditor = new lilToonInspector();
    ToonStandardShaderEditor toonStandardEditor = new ToonStandardShaderEditor();

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        EditorGUILayout.LabelField("lilToon", EditorStyles.largeLabel);
        lilToonEditor.OnGUI(materialEditor, properties);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Toon Standard", EditorStyles.largeLabel);
        toonStandardEditor.OnGUI(materialEditor, properties);
    }
}

