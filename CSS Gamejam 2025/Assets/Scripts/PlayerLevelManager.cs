using System;
using System.Collections.Generic;
using PowerUps;
using TMPro;
using UnityEngine;
using Weapons;

public class PlayerLevelManager : MonoBehaviour
{
    [HideInInspector] public GameManager gameManager;

    [SerializeField] private PlayerMovement movementController;
    [SerializeField] private GameObject deathCollider;
    [SerializeField] private TMP_Text distanceDisplay;
    [SerializeField] private List<GameObject> powerUpPrefabs;

    [SerializeField] public GameObject projectilePrefab;

    [SerializeField] private int fireRate = 10;

    private Vector2 _aim;

    private float _distanceTravelled;

    public List<TeleportBoundary> Boundaries;

    public bool Dead { get; private set; }

    public float PosX => movementController.GetXPos();

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

        if (Time.frameCount % fireRate == 1) ShootProjectile(projectilePrefab, movementController.aim);
    }

    private void ShootProjectile(GameObject prefab, Vector2 vel)
    {
        var obj = Instantiate(prefab);
        obj.transform.position = movementController.transform.position;
        var proj = obj.GetComponent<Projectile>();
        proj.InitialRB(vel);
        proj.MarkInitial();
        proj.SetLevelManager(this);
    }

    private void Respawn()
    {
        Dead = false;
        movementController.Respawn();
    }

    public void PlayerDeath()
    {
        Dead = true;
        movementController.Death(1.0f, 2.0f);
        Invoke(nameof(Respawn), 3.0f);
    }

    public void IncreaseSpeed(int speed)
    {
        movementController.AddSpeed(speed);
        Debug.Log("Added speed");
    }

    public void ReduceSpeed(int speed)
    {
        movementController.AddSpeed(1.0f / speed);
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

    public void PlayerDeathImmediate()
    {
        Dead = true;
        movementController.DeathImmediate(3.0f);
        Invoke(nameof(Respawn), 3.0f);
    }
}