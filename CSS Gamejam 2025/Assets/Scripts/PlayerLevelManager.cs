using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement movementController;

    public void Start()
    {
        var grid = GameObject.Find("Grid");
        gameObject.transform.parent = grid.transform;
        var pos = gameObject.transform.position;
        pos.y = -100.0f * grid.transform.childCount;
        gameObject.transform.position = pos;

        movementController.SetLevelManager(this);
    }

    public void PlayerDeath(Vector2 respawnPos)
    {
        movementController.Teleport(respawnPos);
    }
}