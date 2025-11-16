using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[Serializable]
public class BackgroundGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap backgroundTilemap;
    [SerializeField] private TileBase[] backgroundTiles;

    private int bufferTiles = 5;


    private Camera _camera;
    private readonly HashSet<Vector3Int> _generatedPositions = new();
    private Vector3Int _lastCameraCell = new(int.MinValue, int.MinValue, 0);

    private void Start()
    {
        var cameraParent = GetComponentInParent<Camera>();
        if (cameraParent == null)
        {
            Debug.LogError("No camera found in parent.");
            return;
        }

        _camera = cameraParent;
        GenerateRandomBackground();
    }


    private void Update()
    {
        if (!_camera)
            return;
        var cameraPos = _camera.transform.position;
        var currentCell = backgroundTilemap.WorldToCell(cameraPos);

        if (currentCell == _lastCameraCell)
            return;
        _lastCameraCell = currentCell;
        GenerateRandomBackground();
    }

    private void GenerateRandomBackground()
    {
        var cameraPos = _camera.transform.position;
        var viewHeight = _camera.orthographicSize * 2f;
        var viewWidth = viewHeight * _camera.aspect;

        var minCell = backgroundTilemap.WorldToCell(new Vector3(
            cameraPos.x - viewWidth / 2f - bufferTiles,
            cameraPos.y - viewHeight / 2f - bufferTiles,
            0f));
        var maxCell = backgroundTilemap.WorldToCell(new Vector3(
            cameraPos.x + viewWidth / 2f + bufferTiles,
            cameraPos.y + viewHeight / 2f + bufferTiles,
            0f));

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                if (_generatedPositions.Contains(pos))
                    continue;

                var tile = backgroundTiles[Random.Range(0, backgroundTiles.Length)];
                backgroundTilemap.SetTile(pos, tile);
                _generatedPositions.Add(pos);
            }
        }
    }
}