using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMovement : MonoBehaviour
{
    private static readonly int[] Pattern = { +1, +1, -1, -1 };
    private int _stepIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(UpdateObjects), 0.0f, 1.0f);
    }

    void UpdateObjects()
    {
        var tilemaps = GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            if (tilemap.name != "PrefabMovingPlatform1") continue;
            var pos = tilemap.transform.position;
            pos.y += Pattern[_stepIndex];
            tilemap.transform.position = pos;
            
            _stepIndex = (_stepIndex + 1) % Pattern.Length;
        }
    }
}