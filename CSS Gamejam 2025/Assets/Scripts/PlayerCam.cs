using System;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class PlayerCam : MonoBehaviour
{
    private static readonly List<PlayerCam> Cams = new();

    [SerializeField] private GameObject player;

    public PlayerLevelManager levelManager;
    private Camera _cam;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        Cams.Add(this);
        var divisor = Cams.Count;
        var splits = Convert.ToInt32(Mathf.Log(divisor, 2));
        Debug.Log($"Splits: {splits}");
        var rectWidth = 1.0f;
        var rectHeight = 1.0f;
        for (var i = 0; i < splits; i++)
            if (i % 2 == 0)
                rectHeight /= 2.0f;
            else
                rectWidth /= 2.0f;

        List<TeleportBoundary> boundaries = new();

        TeleportBoundary boundary;
        boundary.Axis = Axis.Y;
        var rowWidth = 0.0f;
        var height = 1.0f - rectHeight;
        foreach (var cam in Cams)
        {
            var rect = new Rect();

            if (1.0f - rowWidth < float.Epsilon)
            {
                rowWidth = 0.0f;
                height -= rectHeight;
                boundary.Start = Cams[0].transform.position.y - _cam.orthographicSize;
                boundary.End = cam.transform.position.y + _cam.orthographicSize;
                boundary.From = Cams[0].levelManager;
                boundary.To = levelManager;
                boundaries.Add(boundary);
            }

            rect.x = rowWidth;
            rect.y = height;
            rect.width = rectWidth;
            rect.height = rectHeight;
            rowWidth += rect.width;

            cam.SetRect(rect);
        }

        if (boundaries.Count == 0) Debug.LogWarning("NO BOUNDARY");
        Projectile.SetBoundaries(new List<TeleportBoundary>(), boundaries);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var pos = transform.position;
        pos.x = player.transform.position.x;
        transform.position = pos;
    }

    private void SetRect(Rect rect)
    {
        _cam.rect = rect;
    }
}