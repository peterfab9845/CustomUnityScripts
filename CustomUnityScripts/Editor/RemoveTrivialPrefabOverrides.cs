using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class RemoveTrivialPrefabOverrides
{
    static List<string> propertiesToRevert = new List<string> {
        "m_LocalPosition.x",
        "m_LocalPosition.y",
        "m_LocalPosition.z",
        "m_LocalRotation.x",
        "m_LocalRotation.y",
        "m_LocalRotation.z",
        "m_LocalRotation.w",
        "m_LocalScale.x",
        "m_LocalScale.y",
        "m_LocalScale.z",
        "m_LocalEulerAnglesHint.x",
        "m_LocalEulerAnglesHint.y",
        "m_LocalEulerAnglesHint.z",
    };

    [MenuItem("Tools/Remove Trivial Overrides from Prefab", false, 155)]
    public static void RemoveTrivialOverrides() {
        foreach (GameObject go in Selection.gameObjects) {
            if (!PrefabUtility.IsOutermostPrefabInstanceRoot(go)) continue;

            PropertyModification[] mods = PrefabUtility.GetPropertyModifications(go);
            if (mods == null) continue;

            List<PropertyModification> modsToKeep = new();
            foreach (var mod in mods) {
                bool keep = true;
                if (mod.target is Transform t) {
                    if (propertiesToRevert.Contains(mod.propertyPath)) {
                        SerializedObject so = new(t);
                        SerializedProperty sp = so.FindProperty(mod.propertyPath);
                        if (sp.propertyType == SerializedPropertyType.Float) {
                            float modValue = float.Parse(mod.value);
                            if (Math.Abs(modValue - sp.floatValue) < 0.0001f) {
                                Debug.Log($"Reverting instance ({go.name}) object ({t.name}) property ({mod.propertyPath}) to parent's value ({sp.floatValue}), override value was ({modValue}) diff=({modValue - sp.floatValue})");
                                keep = false;
                            }
                        }
                        else {
                            Debug.LogWarning($"Instance ({go.name}) object ({t.name}) property ({mod.propertyPath}) overridden from parent value ({sp.floatValue}) to override value ({mod.value}), don't know how to compare");
                        }
                    }
                }
                if (keep) {
                    modsToKeep.Add(mod);
                }
            }
            if (modsToKeep.Count < mods.Length) {
                Undo.RegisterCompleteObjectUndo(go, "Remove trivial overrides");
                PrefabUtility.SetPropertyModifications(go, modsToKeep.ToArray());
            }
        }
    }
}
