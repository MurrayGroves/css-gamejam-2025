using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using PowerUps;
using Random = UnityEngine.Random;

public class PlayerLevelManager : MonoBehaviour
{
    [HideInInspector] public GameManager gameManager;
    
    [SerializeField] private PlayerMovement movementController;
    [SerializeField] private GameObject deathCollider;
    [SerializeField] private TMP_Text distanceDisplay;

    private bool _dead;

    private float _distanceTravelled;
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

    public void IncreaseSpeed(int speed)
    {
        movementController.AddSpeed(speed);
        Debug.Log("Added speed");
    }

    public void ReduceSpeed(int speed)
    {
        movementController.AddSpeed(1.0f/speed);
        Debug.Log("Reduced speed");
    }

    private void SpawnPowerUps()
    {
        for (int i = 0; i < powerUpPrefabs.Count; i++)
        {
            powerUpPrefabs[i].GetComponent<PowerUp>().SetLevelManager(this);
            int num = Random.Range(-290, -300);
            Instantiate(powerUpPrefabs[i], new Vector3(i * 10, num), Quaternion.identity);
        }
        
        Debug.Log("Spawned power ups");
    }

    public void InvertControls()
    {
        movementController.InvertControls();
        Invoke(nameof(RevertControls), 10);
    }
    
    public void RevertControls()
    {
        movementController.RevertControls();
    }

    public void IncreaseGravity(int gravityMultiplier)
    {
        movementController.IncreaseGravity(gravityMultiplier);
        Invoke(nameof(RevertGravity), 10);
    }

    public void RevertGravity()
    {
        movementController.RevertGravity();
    }
}