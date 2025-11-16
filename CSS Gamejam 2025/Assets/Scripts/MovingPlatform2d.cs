using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform2d : MonoBehaviour
{
    [SerializeField] private Vector2 moveAxis = Vector2.up; // Direction to move
    [SerializeField] private float stepSize = 1f;           // Units per step
    [SerializeField] private float stepInterval = 1f;       // Seconds per step
    [SerializeField] private int[] pattern = { +1, +1, -1, -1 }; // Sequence

    private Rigidbody2D _rb;
    private int _stepIndex;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Debug.Log($"RB: {_rb}");
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        InvokeRepeating(nameof(UpdateObjects), 0.0f, 1.0f);
    }


    private void UpdateObjects()
    {
        var tilemaps = GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            if (tilemap.name != "PrefabMovingPlatform1") continue;
            var dir = moveAxis.sqrMagnitude > 0f ? moveAxis.normalized : Vector2.up;
            var delta = dir * stepSize * pattern[_stepIndex];
            _rb.MovePosition(_rb.position + delta);
            _stepIndex = (_stepIndex + 1) % pattern.Length;
        }
    }
}