using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Min(1)] public int maxChunks;
    public Grid grid;
    public Tilemap[] chunkPrefabs;
    public SerializedDictionary<int, int[]> compatibilities = new();
    
    private readonly Camera _camera;
    private readonly List<Tilemap> _spawnedChunks = new();
    private readonly List<int> _spawnedChunkIndices = new();
    private static int count = 0;

    private void Update()
    {
        //ClearOutOfView();
        Generate();
    }

    private void Generate()
    {
        if (count > maxChunks) return;
        Tilemap newChunk;
        
        if (_spawnedChunks.Count > 0)
        {
            var chunkIndex = GetCompatibleChunk();
            newChunk = Instantiate(chunkPrefabs[chunkIndex]);
            _spawnedChunkIndices.Add(chunkIndex);
        }
        else
        {
            newChunk = Instantiate(chunkPrefabs[0]);
            _spawnedChunkIndices.Add(0);
        }
        
        newChunk.CompressBounds();
        
        if (_spawnedChunks.Count > 0)
        {
            var lastChunk = _spawnedChunks[^1];
            var lastWidth = lastChunk.cellBounds.size.x;
            newChunk.transform.position = lastChunk.transform.position + new Vector3(lastWidth, 0);
        }

        newChunk.transform.parent = grid.transform;
        _spawnedChunks.Add(newChunk);
        count++;
    }

    private int GetCompatibleChunk()
    {
        int[] possibleNext = compatibilities[_spawnedChunkIndices[^1]];
        int indexOfNextChunk = Random.Range(0, possibleNext.Length);
        int next = possibleNext[indexOfNextChunk];
        return next;
    }

    private void ClearOutOfView()
    {
        foreach (var chunk in _spawnedChunks)
        {
            if (IsOutOfView(chunk))
            {
                DestroyImmediate(chunk);
            }
        }
    }

    private bool IsOutOfView(Tilemap tilemap)
    {
        tilemap.CompressBounds();
        //tilemap.cellBounds;
        //Vector3 pointOnScreen = _camera.WorldToScreenPoint(bounds.max);
        
        //Is in FOV
        // if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
        //     (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
        // {
        //     Debug.Log("OutOfBounds: " + tilemap.name);
        //     return false;
        // }

        return true;
    }
}