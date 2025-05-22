using UnityEditor;
using UnityEngine;

public class DeleteCollidersFromPrefabs : MonoBehaviour
{
    [MenuItem("Tools/Delete Colliders from Selected Prefabs")]
    static void DeleteColliders()
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

            int count = 0;
            var colliders = root.GetComponentsInChildren<Collider>(true);
            foreach (var col in colliders)
            {
                Object.DestroyImmediate(col, true);
                count++;
            }

            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);

            Debug.Log($"Removed {count} collider(s) from prefab: {path}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}