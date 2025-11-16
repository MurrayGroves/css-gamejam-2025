using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkMetadata : MonoBehaviour
{
    private const int chunkHeight = 16;

    public EdgePoint[] leftPoints;
    public EdgePoint[] rightPoints;
    private readonly int maxScanAirAbove = 16;


    private readonly int playerHeightInTiles = 2;

    public void BakeFromTilemap(Tilemap tilemap)
    {
        if (tilemap == null)
        {
            Debug.LogError("ChunkMetadata.BakeFromTilemap: Tilemap is null.");
            return;
        }

        tilemap.CompressBounds();
        var b = tilemap.cellBounds;

        Debug.Log($"[{name}] has bounds: {b}");
        var minX = b.xMin;
        var maxX = b.xMax - 1;

        leftPoints = CollectEdgePoints(tilemap, minX, b.yMin, chunkHeight);
        rightPoints = CollectEdgePoints(tilemap, maxX, b.yMin, chunkHeight);


        Debug.Log($"[{name}] Collected {leftPoints.Length} left points and {rightPoints.Length} right points.");
    }

    private EdgePoint[] CollectEdgePoints(Tilemap tilemap, int edgeX, int yMin, int yMax)
    {
        var list = new List<EdgePoint>();
        for (var y = yMin; y < yMax; y++)
        {
            var groundPos = new Vector3Int(edgeX, y, 0);
            if (!tilemap.HasTile(groundPos)) continue; // need ground tile to stand on

            var clearance = 0;
            Debug.Log($"{tilemap.name} Searching clearance above edgeX={edgeX} y={y}");
            for (var h = 1; h <= maxScanAirAbove; h++)
            {
                var above = new Vector3Int(edgeX, y + h, 0);
                if (tilemap.HasTile(above))
                {
                    Debug.Log($"{tilemap.name} edgeX={edgeX} y={y} blocked at height {h} by tile at {above}");
                    break;
                }

                clearance++;
            }

            if (clearance < playerHeightInTiles) continue;
            Debug.Log($"{tilemap.name} edgeX={edgeX} y={y} has clearance {clearance}");
            list.Add(new EdgePoint { y = y, clearance = clearance });
        }

        return list.ToArray();
    }

    [Serializable]
    public struct EdgePoint
    {
        public int y; // Tile Y coordinate where player can stand
        public int clearance; // Free vertical tiles above
    }
}