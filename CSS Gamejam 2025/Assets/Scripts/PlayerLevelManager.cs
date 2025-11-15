using System;
using TMPro;
using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement movementController;
    [SerializeField] private GameObject deathCollider;
    [SerializeField] private TMP_Text distanceDisplay;

    private bool _dead;

    private float _distanceTravelled;

    public void Start()
    {
        var grid = GameObject.Find("Grid");
        gameObject.transform.parent = grid.transform;
        var pos = gameObject.transform.position;
        pos.y = -100.0f * grid.transform.childCount;
        gameObject.transform.position = pos;

        movementController.SetLevelManager(this);
    }

    private void FixedUpdate()
    {
        var xPos = movementController.GetXPos();
        if (xPos > _distanceTravelled)
        {
            _distanceTravelled = xPos;
            distanceDisplay.text = $"{Convert.ToInt32(_distanceTravelled)}m travelled";
        }

        deathCollider.transform.position = new Vector2(xPos, deathCollider.transform.position.y);
    }

    private void Respawn()
    {
        _dead = false;
        movementController.Respawn();
    }

    public void PlayerDeath()
    {
        _dead = true;
        movementController.Death();
        Invoke(nameof(Respawn), 3.0f);
    }
}