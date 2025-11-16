using UnityEngine;

namespace PowerUps
{
    public class IncreaseGravity : PowerUp
    {
        private const int Gravity = 20;
        
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("POWER UP: Gravity increased");
            var collidedPlayer = other.GetComponent<PlayerLevelManager>();
            if (Player.gameManager.allPlayers.Count < 2) return;
            Player.gameManager.allPlayers.ForEach(player =>
            {
                if (player != collidedPlayer)
                {
                    player.IncreaseGravity(Gravity);    
                }
            });
            // consume power up
            Destroy(gameObject);
        }
    }
}