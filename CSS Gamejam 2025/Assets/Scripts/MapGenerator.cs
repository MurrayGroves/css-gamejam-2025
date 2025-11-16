using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
    [ItemNotNull] private GameObject[] _chunkPrefabs;

    [NotNull] public GameObject startingPrefab;
    // public Grid grid;

    private readonly Camera _camera;


    private void Start()
    {
        var prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        var chunkPrefabs = new List<GameObject>();
        foreach (var prefabGuid in prefabGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(prefabGuid);
            // if path is in WorldChunks folder:
            if (!path.Contains("WorldChunks"))
            {
                continue;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                continue;
            }


            Debug.Log($"Found chunk prefab: {prefab.name}");
            chunkPrefabs.Add(prefab);
        }

        _chunkPrefabs = chunkPrefabs.ToArray();
        for (var i = 0; i < 20; i++)
        {
            Generate();
        }
    }

    private readonly List<Tilemap> _spawnedChunks = new();
    private readonly List<Transform> _chunkRoots = new();

    private (Transform root, Tilemap tilemap) InstantiateChunk(GameObject prefab)
    {
        Debug.Log($"Instantiating chunk prefab '{prefab.name}' at {transform.position}");
        var root = Instantiate(prefab, transform).transform;
        Debug.Log($"Instantiated chunk prefab '{prefab.name}' as '{root}'");
        root.name = prefab.name;

        var tilemap = root.GetComponentInChildren<Tilemap>();
        if (tilemap == null)
            throw new Exception("Not a tilemap");


        return (root, tilemap);
    }

    private void Generate()
    {
        if (_spawnedChunks.Count == 0) // First chunk should always be the first prefab
        {
            var first = InstantiateChunk(startingPrefab);
            first.tilemap.CompressBounds();
            _chunkRoots.Add(first.root);
            _spawnedChunks.Add(first.tilemap);

            return;
        }

        var choice = GetCompatibleChunkChoice();
        var (root, tilemap) = InstantiateChunk(choice.prefab);

        tilemap.CompressBounds();
        var lastRoot = _chunkRoots[^1];
        var lastTilemap = _spawnedChunks[^1];
        var lastWidth = lastTilemap.cellBounds.size.x;

        // Place to the right of the last chunk
        root.position = lastRoot.position + new Vector3(lastWidth, 0f);
        Debug.Log($"Placed chunk '{tilemap.name}' at {root.position}");


        _chunkRoots.Add(root);
        _spawnedChunks.Add(tilemap);
    }

    private (GameObject prefab, int? matchY) GetCompatibleChunkChoice()
    {
        var prevTilemap = _spawnedChunks[^1]; // last spawned chunk
        var prevMeta = prevTilemap.GetComponent<ChunkMetadata>();

        if (!prevMeta || prevMeta.rightPoints == null || prevMeta.rightPoints.Length == 0)
        {
            return (_chunkPrefabs[Random.Range(0, _chunkPrefabs.Length)], null);
        }


        var candidates = new List<(GameObject prefab, int? matchY)>();
        foreach (var prefab in _chunkPrefabs)
        {
            var meta = prefab.GetComponentInChildren<ChunkMetadata>(true);
            if (meta?.leftPoints == null || meta.leftPoints.Length == 0)
            {
                Debug.LogWarning($"Chunk prefab '{prefab.name}' is missing ChunkMetadata or leftPoints.");
                continue;
            }

            // a chunk can be a candidate so long as:
            // all the possible right edge Y positions from the previous chunk
            // have at least one matching left edge Y position in this chunk

            bool YMatches(ChunkMetadata.EdgePoint point)
            {
                foreach (var metaLeftPoint in meta.leftPoints)
                {
                    // test if there is an intersection in prevMeta.rightPoints + clearance and meta.leftPoints + clearance
                    if (Math.Abs(point.y - metaLeftPoint.y) <= Math.Min(point.clearance, metaLeftPoint.clearance))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (!prevMeta.rightPoints.Any(YMatches))
            {
                Debug.LogWarning($"Chunk prefab '{prefab.name}' is not compatible with '{prevTilemap.name}'");
                continue; // not compatible
            }

            candidates.Add((prefab, null));
        }

        Debug.Log($"Found {candidates.Count} compatible chunk candidates with '{prevTilemap.name}'");
        if (candidates.Count != 0) return candidates[Random.Range(0, candidates.Count)];

        // Fallbacks: prefer with metadata, otherwise any.
        var withMeta = _chunkPrefabs.Where(p => p && p.GetComponentInChildren<ChunkMetadata>(true) != null)
            .ToArray();
        if (withMeta.Length > 0)
            return (withMeta[Random.Range(0, withMeta.Length)], null);

        return (_chunkPrefabs[Random.Range(0, _chunkPrefabs.Length)], null);
    }
}