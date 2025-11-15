using System.Collections.Generic;
using UnityEngine;
using PowerUps;

public class PlayerLevelManager : MonoBehaviour
{
    public GameManager gameManager;
    [SerializeField] private PlayerMovement movementController;
    [SerializeField] private List<PowerUp> powerUps;

    public void Start()
    {
        var grid = GameObject.Find("Grid");
        gameObject.transform.parent = grid.transform;
        var pos = gameObject.transform.position;
        pos.y = -100.0f * grid.transform.childCount;
        gameObject.transform.position = pos;

        movementController.SetLevelManager(this);
        powerUps.ForEach(powerUp => powerUp.SetLevelManager(this));
        gameManager.RegisterPlayerLevelManager(this);
    }

    public void PlayerDeath(Vector2 respawnPos)
    {
        movementController.Teleport(respawnPos);
    }

    public void AddSpeed(int speed)
    {
        movementController.AddSpeed(speed);
    }

    public void ReduceSpeed(int speed)
    {
        movementController.AddSpeed(-speed);
    }
}