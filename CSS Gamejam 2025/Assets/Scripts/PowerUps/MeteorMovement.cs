using UnityEngine;

public class MeteorMovement : MonoBehaviour
{
    public float speed = 1f;
    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }
}
