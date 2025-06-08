// Based on http://www.tallior.com/find-missing-references-unity/

// Put this script in Assets/Editor/ so it doesn't get built with the project!

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

public static class CheckMissingReferences
{
    [MenuItem("Tools/Show Missing Object References in scene", false, 50)]
    public static void FindMissingReferencesInCurrentScene()
    {
        var objects = Object.FindObjectsOfType<GameObject> ();
        FindMissingReferences(EditorSceneManager.GetActiveScene().name, objects);
    }

    [MenuItem("Tools/Show Missing Object References in all enabled scenes", false, 51)]
    public static void FindMissingReferencesInAllScenes()
    {
        foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
        {
            EditorSceneManager.OpenScene(scene.path);
            var objects = Object.FindObjectsOfType<GameObject> ();
            FindMissingReferences(scene.path, objects);
        }
    }

    [MenuItem("Tools/Show Missing Object References in assets", false, 52)]
    public static void FindMissingReferencesInAssets()
    {
        var allAssets = AssetDatabase.GetAllAssetPaths();
        var objs = allAssets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject).Where(a => a != null).ToArray();

        FindMissingReferences("Project", objs);
    }

    // Find missing object references in objects
    public static void FindMissingReferences(string sceneName, GameObject[] objects)
    {
        Debug.Log($"Checking for missing references in Scene=({sceneName})");
        foreach (var go in objects)
        {
            var components = go.GetComponents<Component> ();

            foreach (var c in components)
            {
                var so = new SerializedObject(c);
                var sp = so.GetIterator();

                while (sp.Next(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
                        {
                            Debug.LogError($"Missing reference: Scene=({sceneName}) Object=({FullObjectPath(go)}) Component=({c}) has missing reference in Property=({sp.propertyPath}) Name=({sp.displayName})", c);
                        }
                    }
                }
                var animator = c as Animator;
                if (animator != null) {
                    CheckAnimatorReferences(sceneName, animator);
                }
            }
        }
    }

    static void CheckAnimatorReferences(string sceneName, Animator animator)
    {
        if (animator.runtimeAnimatorController == null)
        {
            return;
        }

        Debug.Log($"Checking for missing references in Scene=({sceneName}) Animator=({FullObjectPath(animator.gameObject)})");

        foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
        {
            var curveBindings = AnimationUtility.GetCurveBindings(ac);
            var objectReferenceCurveBindings = AnimationUtility.GetObjectReferenceCurveBindings(ac);
            var allBindings = curveBindings.Concat(objectReferenceCurveBindings);

            // Find animation bindings which bind to a nonexistent object or property
            foreach (var binding in allBindings)
            {
                // TODO: try to get the bound object/property in a nicer way, skip this text stuff since it doesn't always work
                // maybe see https://github.com/gydisme/Unity-Game-Framwork/blob/master/Assets/Editor/CustomEditor/Monitor4AnimationCurve/Monitor4AnimationCurve.cs
                Transform t = animator.gameObject.transform.Find(binding.path);
                if (t == null) {
                    Debug.LogError($"Missing reference: Scene=({sceneName}) Animator=({FullObjectPath(animator.gameObject)}) Clip=({ac.name}) tries to reference Path=({binding.path}) Type=({binding.type}) Property=({binding.propertyName}), but the object is missing", animator);
                } else {
                    // TODO: check property itself too
                    //Debug.Log($"Reference: Scene=({sceneName}) Animator=({FullObjectPath(animator.gameObject)}) Clip=({ac.name}) tries to reference Path=({binding.path}) Type=({binding.type}) Property=({binding.propertyName})", animator);
                    if (false) {
                        Debug.LogError($"Missing reference: Scene=({sceneName}) Animator=({FullObjectPath(animator.gameObject)}) Clip=({ac.name}) tries to reference Path=({binding.path}) Type=({binding.type}) Property=({binding.propertyName}), but that property is missing from the object", animator);
                    }
                }
            }

            // TODO: something with objectReferenceCurveBindings instead? It would be nice to be able to get the object reference being animated
            // Find when the animation clip would animate an object reference property to a missing object
            var so = new SerializedObject(ac);
            var sp = so.GetIterator();

            while (sp.Next(true)) {
                if (sp.propertyType == SerializedPropertyType.ObjectReference) {
                    if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0) {
                        Debug.LogError($"Missing reference: Scene=({sceneName}) Animator=({FullObjectPath(animator.gameObject)}) Clip=({ac.name}) tries to animate an object reference to a missing object. Try turning on preview and looking for missing. Maybe helpful junk: PropertyPath=({sp.propertyPath}) DisplayName=({sp.displayName})", animator);
                    }
                }
            }
        }
    }

    static string FullObjectPath(GameObject go)
    {
        return go.transform.parent == null ? go.name : FullObjectPath(go.transform.parent.gameObject) + "/" + go.name;
    }
}
