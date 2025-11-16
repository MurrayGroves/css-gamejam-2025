using UnityEngine;

namespace PowerUps
{
    public class IncreaseGravity : PowerUp
    {
        private const int Gravity = 20;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var collidedPlayer = other.GetComponentInParent<PlayerLevelManager>();
            if (gameManager.allPlayers.Count < 2) return;
            gameManager.allPlayers.ForEach(player =>
            {
                if (player != collidedPlayer)
                {
                    player.IncreaseGravity(Gravity);
                    Notify(collidedPlayer);
                    Debug.Log("POWER UP: Gravity increased");
                }
            });
            // consume power up
            Destroy(gameObject);
    
        }
    }
}