using System.Collections.Generic;
using UnityEngine;
using PowerUps;

public class PlayerLevelManager : MonoBehaviour
{
    [HideInInspector] public GameManager gameManager;
    
    [SerializeField] private PlayerMovement movementController;
    [SerializeField] private List<GameObject> powerUpPrefabs;
    
    public void Start()
    {
        var grid = GameObject.Find("Grid");
        gameObject.transform.parent = grid.transform;
        var pos = gameObject.transform.position;
        pos.y = -100.0f * grid.transform.childCount;
        gameObject.transform.position = pos;

        gameManager = FindFirstObjectByType<GameManager>();
        gameManager.RegisterPlayerLevelManager(this);
        movementController.SetLevelManager(this);
        SpawnPowerUps();
    }

    public void PlayerDeath(Vector2 respawnPos)
    {
        movementController.Teleport(respawnPos);
    }

    public void AddSpeed(int speed)
    {
        movementController.AddSpeed(speed);
        Debug.Log("Added speed");
    }

    public void ReduceSpeed(int speed)
    {
        movementController.AddSpeed(-speed);
        Debug.Log("Reduced speed");
    }

    private void SpawnPowerUps()
    {
        powerUpPrefabs.ForEach(powerUp =>
        {
            powerUp.GetComponent<PowerUp>().SetLevelManager(this);
            Instantiate(powerUp, new Vector3(0, -198), Quaternion.identity);
        });
        Debug.Log("Spawned power ups");
    }
}