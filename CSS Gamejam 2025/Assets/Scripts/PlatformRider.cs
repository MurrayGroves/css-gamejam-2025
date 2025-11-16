using UnityEngine;

public class PlatformRider : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(playerTag))
            collision.transform.SetParent(transform);
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(playerTag))
            collision.transform.SetParent(null);
    }
}