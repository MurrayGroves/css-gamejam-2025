using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    private static readonly List<PlayerCam> Cams = new();

    [SerializeField] private GameObject player;

    public Camera _cam;

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

        var rowWidth = 0.0f;
        var height = 0.0f;
        foreach (var cam in Cams)
        {
            var rect = new Rect();

            if (1.0f - rowWidth < float.Epsilon)
            {
                rowWidth = 0.0f;
                height += rectHeight;
            }

            rect.x = rowWidth;
            rect.y = height;
            rect.width = rectWidth;
            rect.height = rectHeight;
            rowWidth += rect.width;

            cam.SetRect(rect);
        }
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