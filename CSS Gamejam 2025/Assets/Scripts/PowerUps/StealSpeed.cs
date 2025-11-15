using UnityEngine;

namespace PowerUps
{
    public class StealSpeed : MonoBehaviour
    {
        private GameObject player1;
        private GameObject player2;
        
        private const int SpeedMultiplier = 10;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Collision detected");
            var players = FindObjectsByType<PlayerMovement>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
            player1 = players[0].gameObject;
            player2 = players[1].gameObject;
            
            if (other.gameObject == player1)
            {
                ApplyPowerUp(player2, player1);
            }
            else if (other.gameObject == player2)
            {
                ApplyPowerUp(player1, player2);
            }
        }

        private static void ApplyPowerUp(GameObject from, GameObject to)
        {
            var fromPhysics = from.GetComponent<Rigidbody2D>();
            fromPhysics.linearDamping = SpeedMultiplier;
            var toPhysics = to.GetComponent<Rigidbody2D>();
            toPhysics.linearDamping = -SpeedMultiplier;
            Debug.Log("Power up applied");
        }
    }
}