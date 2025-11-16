using UnityEngine;

namespace PowerUps
{
    public class InvertControls : PowerUp
    {
        protected void OnTriggerEnter2D(Collider2D other)
        {
            var collidedPlayer = other.gameObject.GetComponent<PlayerLevelManager>();
            if (gameManager.allPlayers.Count < 2) return;
            gameManager.allPlayers.ForEach(player =>
            {
                if (player != collidedPlayer)
                {
                    player.InvertControls();
                    Debug.Log("POWER UP: Inverted controls");
                }
            });
            // consume power up
            Destroy(gameObject);
        }
    }
}