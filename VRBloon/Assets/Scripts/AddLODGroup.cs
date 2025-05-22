using UnityEditor;
using UnityEngine;

public class AddLODGroupToPrefabs : MonoBehaviour
{
    [MenuItem("Tools/Add LOD Group (Cull at 10%) to Selected Prefabs")]
    static void AddLODGroup()
    {
        var selected = Selection.GetFiltered<GameObject>(SelectionMode.Assets);

        foreach (var prefab in selected)
        {
            var path = AssetDatabase.GetAssetPath(prefab);
            var root = PrefabUtility.LoadPrefabContents(path);

            if (root == null)
            {
                Debug.LogWarning($"Could not load prefab at {path}");
                continue;
            }

            var renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                Debug.LogWarning($"No renderers found in prefab: {path}");
                PrefabUtility.UnloadPrefabContents(root);
                continue;
            }

            var lodGroup = root.GetComponent<LODGroup>();
            if (lodGroup == null)
                lodGroup = root.AddComponent<LODGroup>();

            // LOD setup: 0 to 0.1 = visible; below 0.1 = culled
            LOD[] lods = new LOD[1];
            lods[0] = new LOD(0.025f, renderers);
            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();

            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);

            Debug.Log($"Added LODGroup to prefab: {path}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}