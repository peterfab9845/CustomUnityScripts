using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaterialPropertyCopier : PropertyEditorWindow
{
    static EditorWindow window;
    static Material src;
    static Material tgt;
    static bool refreshSrc;
    static bool refreshTgt;
    SerializedObject srcSerialized;
    SerializedObject tgtSerialized;
    Vector2 srcScrollPos;
    Vector2 tgtScrollPos;

    [MenuItem("Tools/Material Property Copier", false, 121)]
    public static void Init() {
        window = GetWindow<MaterialPropertyCopier>("Material Property Copier", true);

        Material sel = Selection.activeObject as Material;
        if (sel != null) {
            src = sel;
            refreshSrc = true;
        }
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Swap source and target")) {
            (src, tgt) = (tgt, src);
            refreshSrc = true;
            refreshTgt = true;
        }
        
        EditorGUILayout.BeginHorizontal();
        DrawSrc();
        DrawTgt();
        EditorGUILayout.EndHorizontal();
    }

    public void DrawSrc() {
        EditorGUILayout.BeginVertical(GUILayout.Width(Screen.width/2));

        EditorGUI.BeginChangeCheck();
        src = (Material) EditorGUILayout.ObjectField("Source", src, typeof(Material), true);
        refreshSrc |= EditorGUI.EndChangeCheck();
        refreshSrc |= GUILayout.Button("Read serialized properties");

        if (refreshSrc) {
            srcSerialized = (src == null) ? null : new SerializedObject(src);
            refreshSrc = false;
        }

        EditorGUI.BeginDisabledGroup(srcSerialized == null);
        if (GUILayout.Button("Write serialized properties")) {
            srcSerialized.ApplyModifiedProperties();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(src == null || tgt == null || srcSerialized == null || tgtSerialized == null);
        if (GUILayout.Button("Merge source into target, skip conflicts")) {
            CopySerializedKeywords(srcSerialized, tgtSerialized, false);
            CopySerializedStringTagMap(srcSerialized, tgtSerialized, false, false);
            CopySerializedSavedProperties(srcSerialized, tgtSerialized, false, false);
        }
        if (GUILayout.Button("Merge source into target, overwrite conflicts")) {
            CopySerializedKeywords(srcSerialized, tgtSerialized, false);
            CopySerializedRenderQueue(srcSerialized, tgtSerialized);
            CopySerializedStringTagMap(srcSerialized, tgtSerialized, false, true);
            CopySerializedSavedProperties(srcSerialized, tgtSerialized, false, true);
        }
        // if (GUILayout.Button("Set shader, copy mostly correctly with overwrite")) {
        //     CopySerializedStringTagMap(srcSerialized, tgtSerialized, false, true);
        //     tgtSerialized.ApplyModifiedProperties(); // ugly
        //     tgt.shader = src.shader;
        //     tgt.CopyMatchingPropertiesFromMaterial(src);
        // }
        // if (GUILayout.Button("CopyMatchingPropertiesFromMaterial to Target")) {
        //     // Keywords: Merge correctly (target ends up with all keywords, even if not valid for shader)
        //     // Render queue: Set from source
        //     // String tag map: (no action)
        //     // Serialized properties: Copy properties which already exist in the target (even if not valid for its current shader)
        //     // Runtime properties: Copy properties which already exist in the target (even if not valid for its current shader)
        //     tgt.CopyMatchingPropertiesFromMaterial(src);
        // }
        // if (GUILayout.Button("CopyPropertiesFromMaterial to Target")) {
        //     // Keywords: Replace with source
        //     // Render queue: Replace with source
        //     // String tag map: Replace with source
        //     // Serialized properties: Replace with source (including deleting properties)
        //     // Runtime properties: Replace with source
        //     // Completely overwrites everything in tgt with src, even though it breaks the target
        //     // If you do this and also change the shader, the result is identical to the src (?)
        //     tgt.CopyPropertiesFromMaterial(src);
        // }
        // if (GUILayout.Button("CopySerializedShader to Target")) {
        //     CopySerializedShader(srcSerialized, tgtSerialized);
        // }
        // if (GUILayout.Button("CopySerializedKeywords to Target")) {
        //     CopySerializedKeywords(srcSerialized, tgtSerialized, clear: false);
        // }
        // if (GUILayout.Button("CopySerializedRenderQueue to Target")) {
        //     CopySerializedRenderQueue(srcSerialized, tgtSerialized);
        // }
        // if (GUILayout.Button("CopySerializedStringTagMap to Target")) {
        //     CopySerializedStringTagMap(srcSerialized, tgtSerialized, clearKeys: false, overwriteValues: true);
        // }
        // if (GUILayout.Button("CopySerializedSavedProperties to Target")) {
        //     CopySerializedSavedProperties(srcSerialized, tgtSerialized, clearKeys: false, overwriteValues: true);
        // }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        srcScrollPos = EditorGUILayout.BeginScrollView(srcScrollPos);
        if (srcSerialized != null) {
            DrawSerializedObject(srcSerialized, skipStart: true, showHidden: true);
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    public void DrawTgt() {
        EditorGUILayout.BeginVertical(GUILayout.Width(Screen.width/2));

        EditorGUI.BeginChangeCheck();
        tgt = (Material) EditorGUILayout.ObjectField("Target", tgt, typeof(Material), true);
        refreshTgt |= EditorGUI.EndChangeCheck();
        refreshTgt |= GUILayout.Button("Read serialized properties");
        
        if (refreshTgt) {
            tgtSerialized = (tgt == null) ? null : new SerializedObject(tgt);
            refreshTgt = false;
        }

        EditorGUI.BeginDisabledGroup(tgtSerialized == null);
        if (GUILayout.Button("Write serialized properties")) {
            tgtSerialized.ApplyModifiedProperties();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();

        tgtScrollPos = EditorGUILayout.BeginScrollView(tgtScrollPos);
        if (tgtSerialized != null) {
            DrawSerializedObject(tgtSerialized, skipStart: true, showHidden: true);
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    public void CopySerializedShader(SerializedObject src, SerializedObject tgt) {
        SerializedProperty srcShader = src.FindProperty("m_Shader");
        SerializedProperty tgtShader = tgt.FindProperty("m_Shader");
        tgtShader.objectReferenceValue = srcShader.objectReferenceValue;
    }

    // Keywords are automatically moved between Valid and Invalid based on the material's shader
    public void CopySerializedKeywords(SerializedObject src, SerializedObject tgt, bool clear) {
        SerializedProperty srcValidKeywords = src.FindProperty("m_ValidKeywords");
        SerializedProperty srcInvalidKeywords = src.FindProperty("m_InvalidKeywords");
        SerializedProperty tgtValidKeywords = tgt.FindProperty("m_ValidKeywords");
        SerializedProperty tgtInvalidKeywords = tgt.FindProperty("m_InvalidKeywords");

        var srcKeywords = new HashSet<string>();
        for (int i = 0; i < srcValidKeywords.arraySize; i++) {
            srcKeywords.Add(srcValidKeywords.GetArrayElementAtIndex(i).stringValue);
        }
        for (int i = 0; i < srcInvalidKeywords.arraySize; i++) {
            srcKeywords.Add(srcInvalidKeywords.GetArrayElementAtIndex(i).stringValue);
        }

        if (clear) {
            tgtValidKeywords.ClearArray();
            tgtInvalidKeywords.ClearArray();
        } else {
            var tgtKeywords = new HashSet<string>();
            for (int i = 0; i < tgtValidKeywords.arraySize; i++) {
                tgtKeywords.Add(tgtValidKeywords.GetArrayElementAtIndex(i).stringValue);
            }
            for (int i = 0; i < tgtInvalidKeywords.arraySize; i++) {
                tgtKeywords.Add(tgtInvalidKeywords.GetArrayElementAtIndex(i).stringValue);
            }

            srcKeywords.ExceptWith(tgtKeywords);
        }

        foreach (string s in srcKeywords) {
            // arbitrarily insert into ValidKeywords, they get automatically arranged
            tgtValidKeywords.InsertArrayElementAtIndex(tgtValidKeywords.arraySize);
            tgtValidKeywords.GetArrayElementAtIndex(tgtValidKeywords.arraySize-1).stringValue = s;
        }
    }

    public void CopySerializedRenderQueue(SerializedObject src, SerializedObject tgt) {
        SerializedProperty srcRenderQueue = src.FindProperty("m_CustomRenderQueue");
        SerializedProperty tgtRenderQueue = tgt.FindProperty("m_CustomRenderQueue");
        tgtRenderQueue.intValue = srcRenderQueue.intValue;
    }

    public void CopySerializedStringTagMap(SerializedObject src, SerializedObject tgt, bool clearKeys, bool overwriteValues) {
        SerializedProperty srcStringTagMap = src.FindProperty("stringTagMap");
        SerializedProperty tgtStringTagMap = tgt.FindProperty("stringTagMap");

        CopySerializedMap(srcStringTagMap, tgtStringTagMap, clearKeys, overwriteValues);
    }

    public void CopySerializedSavedProperties(SerializedObject src, SerializedObject tgt, bool clearKeys, bool overwriteValues) {
        SerializedProperty srcSavedProperties = src.FindProperty("m_SavedProperties");
        SerializedProperty tgtSavedProperties = tgt.FindProperty("m_SavedProperties");
        
        SerializedProperty srcTexEnvs = srcSavedProperties.FindPropertyRelative("m_TexEnvs");
        SerializedProperty srcInts    = srcSavedProperties.FindPropertyRelative("m_Ints");
        SerializedProperty srcFloats  = srcSavedProperties.FindPropertyRelative("m_Floats");
        SerializedProperty srcColors  = srcSavedProperties.FindPropertyRelative("m_Colors");
        SerializedProperty tgtTexEnvs = tgtSavedProperties.FindPropertyRelative("m_TexEnvs");
        SerializedProperty tgtInts    = tgtSavedProperties.FindPropertyRelative("m_Ints");
        SerializedProperty tgtFloats  = tgtSavedProperties.FindPropertyRelative("m_Floats");
        SerializedProperty tgtColors  = tgtSavedProperties.FindPropertyRelative("m_Colors");

        CopySerializedMap(srcTexEnvs, tgtTexEnvs, clearKeys, overwriteValues);
        CopySerializedMap(srcInts, tgtInts, clearKeys, overwriteValues);
        CopySerializedMap(srcFloats, tgtFloats, clearKeys, overwriteValues);
        CopySerializedMap(srcColors, tgtColors, clearKeys, overwriteValues);
    }

    public void CopySerializedMap(SerializedProperty src, SerializedProperty tgt, bool clearKeys, bool overwriteValues) {
        var srcEntries = new Dictionary<string, SerializedProperty>();
        for (int i = 0; i < src.arraySize; i++) {
            var elem = src.GetArrayElementAtIndex(i);
            var key = elem.FindPropertyRelative("first").stringValue;
            var value = elem.FindPropertyRelative("second");
            srcEntries.Add(key, value);
        }

        var tgtEntries = new Dictionary<string, SerializedProperty>();
        if (clearKeys) {
            tgt.ClearArray();
        } else {
            for (int i = 0; i < tgt.arraySize; i++) {
                var elem = tgt.GetArrayElementAtIndex(i);
                var key = elem.FindPropertyRelative("first").stringValue;
                var value = elem.FindPropertyRelative("second");
                tgtEntries.Add(key, value);
            }
        }

        foreach (var item in srcEntries) {
            if (tgtEntries.ContainsKey(item.Key)) {
                if (overwriteValues) {
                    CopySerializedProperty(item.Value, tgtEntries[item.Key]);
                }
            } else {
                tgt.InsertArrayElementAtIndex(tgt.arraySize);
                var tgtElement = tgt.GetArrayElementAtIndex(tgt.arraySize-1);
                tgtElement.FindPropertyRelative("first").stringValue = item.Key;
                CopySerializedProperty(item.Value, tgtElement.FindPropertyRelative("second"));
            }
        }
    }

    public void CopySerializedProperty(SerializedProperty src, SerializedProperty tgt) {
        if (src.propertyType == SerializedPropertyType.Generic && src.type == "UnityTexEnv") {
            SerializedProperty iterator = src.Copy();
            SerializedProperty end = iterator.GetEndProperty();
            iterator.Next(enterChildren: true); // get into the children of the current property
            do {
                CopySerializedProperty(iterator, tgt.FindPropertyRelative(iterator.name));
            } while (iterator.Next(enterChildren: false) && !SerializedProperty.EqualContents(iterator, end));
        } else {
            tgt.boxedValue = src.boxedValue;
        }
    }
}
