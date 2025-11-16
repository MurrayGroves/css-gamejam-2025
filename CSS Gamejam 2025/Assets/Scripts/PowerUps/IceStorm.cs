using UnityEngine;

namespace PowerUps
{
    public class IceStorm : PowerUp
    {
        private const int Speed = 20;
        
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("POWER UP: Ice storm");
            var collidedPlayer = other.GetComponent<PlayerLevelManager>();
            if (Player.gameManager.allPlayers.Count < 2) return;
            Player.gameManager.allPlayers.ForEach(player =>
            {
                if (player != collidedPlayer)
                {
                    player.IncreaseSpeed(Speed);
                }
            });
            // consume power up
            Destroy(gameObject);
        }
    }
}