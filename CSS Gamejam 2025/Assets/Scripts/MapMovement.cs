using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMovement : MonoBehaviour
{
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
            var vector3 = tilemap.transform.position;
            vector3.y = vector3.y == 0 ? 1 : 0;
            tilemap.transform.position = vector3;
        }
    }
}