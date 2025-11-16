using UnityEngine;

namespace PowerUps
{
    public class StealSpeed : PowerUp
    {
        private const int Speed = 5;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var collidedPlayer = other.GetComponentInParent<PlayerLevelManager>();
            collidedPlayer?.IncreaseSpeed(Speed);
            
            if (gameManager.allPlayers.Count < 2) return;
            gameManager.allPlayers.ForEach(player =>
            {
                if (player != collidedPlayer)
                {
                    player.ReduceSpeed(Speed);
                    Debug.Log("POWER UP: Stealing speed");
                }
            });
            // consume power up
            Destroy(gameObject);
        }
    }
}