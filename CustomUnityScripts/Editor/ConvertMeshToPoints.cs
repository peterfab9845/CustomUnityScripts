using UnityEngine;
using UnityEditor;

public class ConvertMeshToPoints {

    [MenuItem("CONTEXT/MeshFilter/Convert to MeshTopology.Points", false, 100)]
    private static void ConvertToPoints(MenuCommand command)
    {
        MeshFilter filter = (MeshFilter) command.context;
        Mesh sharedMesh = filter.sharedMesh;
        Mesh m = Object.Instantiate(sharedMesh);
        m.name = sharedMesh.name + "_points";
        filter.sharedMesh = m;
        for (int i = 0; i < m.subMeshCount; i++)
        {
            Debug.Log($"Submesh ({i}) original MeshTopology is {m.GetTopology(i)}");
            int[] indices = m.GetIndices(i);
            m.SetIndices(indices, MeshTopology.Points, i);
        }
        AssetDatabase.CreateAsset(m, $"Assets/{m.name}.mesh");
        AssetDatabase.SaveAssets();
    }
}
