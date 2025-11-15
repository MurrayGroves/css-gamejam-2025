using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGeneration : MonoBehaviour
{
    [Header("Level Prefabs")] [Tooltip("The list of prefabs to choose from")]
    public GameObject[] levelPrefabs;

    [Header("Generation Settings")] [Tooltip("The total number of pieces to spawn")]
    public int numberOfPieces = 10;


    [Header("Generation Settings")] [Tooltip("The Tilemap in the scene to build the level onto")]
    public Tilemap targetTilemap;

    void Start()
    {
        if (targetTilemap == null)
        {
            Debug.LogError("Target Tilemap is not set");
            return;
        }

        if (levelPrefabs.Length == 0)
        {
            Debug.LogWarning("No level prefabs assigned.");
            return;
        }


        targetTilemap.ClearAllTiles();
        GenerateLevel();
    }

    void GenerateLevel()
    {
        int nextAvailableCellX = 0;

        for (var i = 0; i < numberOfPieces; i++)
        {
            var randomIndex = Random.Range(0, levelPrefabs.Length);
            var prefabToSpawn = levelPrefabs[randomIndex];


            var prefabTilemap = prefabToSpawn.GetComponentInChildren<Tilemap>();
            if (prefabTilemap == null)
            {
                Debug.LogError($"Prefab '{prefabToSpawn.name}' has no Tilemap component!");
                continue;
            }


            prefabTilemap.CompressBounds();
            var prefabBounds = prefabTilemap.cellBounds;

            // get tiles inside prefab tilemap
            TileBase[] prefabTiles = prefabTilemap.GetTilesBlock(prefabBounds);


            var destPosition = new Vector3Int(nextAvailableCellX - prefabBounds.xMin, prefabBounds.yMin,
                prefabBounds.zMin);
            var destBounds = new BoundsInt(destPosition, prefabBounds.size);


            targetTilemap.SetTilesBlock(destBounds, prefabTiles);


            nextAvailableCellX += prefabBounds.size.x;
            int gap = Random.Range(0, 3);
            // having gaps between tiles breaks the tile rules applying
            // so consider if worth it / don't apply all the time?
            nextAvailableCellX += gap;
        }

        targetTilemap.RefreshAllTiles();
    }
}