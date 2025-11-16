using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class ChunkConnectionsBaker
{
    [MenuItem("Tools/Map/Bake Chunk Metadata For Prefabs")]
    private static void BakePrefabs()
    {
        
        // only do selected ones if any selected
        var selection = Selection.objects
            .Select(AssetDatabase.GetAssetPath)
            .Where(p => !string.IsNullOrEmpty(p))
            .ToArray();

        string[] prefabGuids;
        if (selection.Length > 0)
        {
            prefabGuids = selection
                .Select(AssetDatabase.AssetPathToGUID)
                .Where(g =>
                    PrefabUtility.GetPrefabAssetType(
                        AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(g))) !=
                    PrefabAssetType.NotAPrefab)
                .ToArray();
        }
        else
        {
            prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        }

        
        int baked = 0, skipped = 0;

        foreach (var guid in prefabGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.Contains("WorldChunks"))
            {
                continue;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                skipped++;
                continue;
            }


            // Instantiate a temporary instance to read/modify components.
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                var tilemap = instance.GetComponentInChildren<Tilemap>(true);
                if (tilemap == null)
                {
                    skipped++;
                    continue;
                }
                
                
                var meta = tilemap.GetComponent<ChunkMetadata>() ??
                           instance.GetComponentInChildren<ChunkMetadata>(true);
                if (meta == null)
                {
                    meta = tilemap.gameObject.AddComponent<ChunkMetadata>();
                }

                Undo.RecordObject(meta, "Bake Chunk Metadata");
                meta.BakeFromTilemap(tilemap);
                EditorUtility.SetDirty(meta);
                PrefabUtility.RecordPrefabInstancePropertyModifications(meta);
                
                PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
                baked++;
            }
            finally
            {
                if (instance != null)
                    Object.DestroyImmediate(instance);
            }
        }

        Debug.Log($"ChunkConnectionsBaker: Baked {baked} prefab(s), skipped {skipped}.");
    }
}