using UnityEngine;

namespace PowerUps
{
    public class IceStorm : PowerUp
    {
        private const int Speed = 20;

        private void OnTriggerEnter2D(Collider2D other)
        {
            var collidedPlayer = other.GetComponentInParent<PlayerLevelManager>();
            if (gameManager.allPlayers.Count < 2) return;
            gameManager.allPlayers.ForEach(player =>
            {
                if (player != collidedPlayer)
                {
                    player.IncreaseSpeed(Speed);
                    Debug.Log("POWER UP: Ice storm");
                    Notify(collidedPlayer);
                    Destroy(gameObject);
                }
            });
            // consume power up
            Destroy(gameObject);
        }
    }
}