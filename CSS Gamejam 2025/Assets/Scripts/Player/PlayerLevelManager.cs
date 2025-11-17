using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Weapons;

public class PlayerLevelManager : MonoBehaviour
{
    private static Dictionary<Type, Action<PlayerLevelManager>> _powerupReset = new();
    [HideInInspector] public GameManager gameManager;
    [SerializeField] private GameObject deathCollider;
    [SerializeField] private GameObject ceiling;
    [SerializeField] private TMP_Text distanceDisplay;
    [SerializeField] public TMP_Text powerupDisplay;
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public GameObject grenadePrefab;


    [SerializeField] private int fireRate = 10;

    private Vector2 _aim;

    private float _distanceTravelled;

    private int _fireCounter = 1;

    private Dictionary<Type, float> _powerupTimers = new();

    public List<TeleportBoundary> Boundaries;

    public PlayerMovement MovementController { get; private set; }

    public bool Dead { get; private set; }

    public float PosX => MovementController.GetXPos();

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gameManager.RegisterPlayerLevelManager(this);
        MovementController = GetComponentInChildren<PlayerMovement>();
    }

    private void Start()
    {
        var grid = GameObject.Find("/Grid");
        gameObject.transform.parent = grid.transform;
        var pos = gameObject.transform.position;
        pos.y = -100.0f * grid.transform.childCount;
        gameObject.transform.position = pos;
        MovementController.SetLevelManager(this);
        //SpawnPowerUps();
    }

    private void FixedUpdate()
    {
        var xPos = MovementController.GetXPos();
        if (xPos > _distanceTravelled)
        {
            _distanceTravelled = xPos;
            distanceDisplay.text = $"{Convert.ToInt32(_distanceTravelled)}m travelled";
        }

        deathCollider.transform.position = new Vector2(xPos, deathCollider.transform.position.y);
        ceiling.transform.position = new Vector2(xPos, ceiling.transform.position.y);

        if (Time.frameCount % fireRate == 1)
        {
            _fireCounter = 1;
            ShootProjectile(projectilePrefab, MovementController.aim);
        }

        _fireCounter++;
    }

    public void ShootProjectile(GameObject prefab, Vector2 vel)
    {
        var obj = Instantiate(prefab);
        obj.transform.position = MovementController.transform.position;
        var proj = obj.GetComponent<Projectile>();
        proj.prefab = prefab;
        proj.InitialRB(vel);
        proj.MarkInitial();
        proj.SetLevelManager(this);
    }

    private void Respawn()
    {
        Dead = false;
        MovementController.Respawn();
    }

    public void PlayerDeath()
    {
        Dead = true;
        MovementController.Death(1.0f, 2.0f);
        Invoke(nameof(Respawn), 3.0f);
    }

    public float GetSpeed()
    {
        return MovementController.GetSpeed();
    }

    public void SetSpeed(float speed)
    {
        MovementController.SetSpeed(speed);
    }

    public void Teleport(Vector2 pos)
    {
        MovementController.Teleport(pos);
    }

    public void InvertControls()
    {
        MovementController.InvertControls();
    }

    public void RevertControls()
    {
        MovementController.RevertControls();
    }

    public void PlayerDeathImmediate()
    {
        Dead = true;
        MovementController.DeathImmediate(3.0f);
        Invoke(nameof(Respawn), 3.0f);
    }

    public void SetFriction(float friction)
    {
        MovementController.GetComponent<Rigidbody2D>().sharedMaterial.friction = friction;
    }

    public float GetFriction()
    {
        return MovementController.GetComponent<Rigidbody2D>().sharedMaterial.friction;
    }
}