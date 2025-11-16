using UnityEngine;

public class MeteorMovement : MonoBehaviour
{
    public float speed = 1f;

    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        var collidedPlayer = other.GetComponentInParent<PlayerLevelManager>();
        collidedPlayer.PlayerDeathImmediate();
        Destroy(gameObject);
    }
}
