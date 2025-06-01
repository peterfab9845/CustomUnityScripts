// Put this script in Assets/Editor/ so it doesn't get built with the project!

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
using VRC.SDK3.Dynamics.PhysBone.Components;

public static class SquishPhysboneCurves {

    [MenuItem("CONTEXT/VRCPhysBone/Squish Physbone Curves", false, 50)]
    public static void SquishCurves(MenuCommand menuCommand)
    {
        VRCPhysBone physBone = menuCommand.context as VRCPhysBone;
        Debug.Log("hello world" + physBone.limitRotationYCurve);
    }

}
