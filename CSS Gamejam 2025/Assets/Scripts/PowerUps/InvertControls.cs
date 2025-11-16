using UnityEngine;

namespace PowerUps
{
    public class InvertControls : PowerUp
    {
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("POWER UP: Inverted controls");
            var collidedPlayer = other.GetComponent<PlayerLevelManager>();
            if (Player.gameManager.allPlayers.Count < 2) return;
            Player.gameManager.allPlayers.ForEach(player =>
            {
                if (player != collidedPlayer)
                {
                    player.InvertControls();
                }
            });
            // consume power up
            Destroy(gameObject);
        }
    }
}