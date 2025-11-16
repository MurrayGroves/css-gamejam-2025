using UnityEngine;

namespace PowerUps
{
    public class InvertControls : PowerUp
    {
        protected void OnTriggerEnter2D(Collider2D other)
        {
            var collidedPlayer = other.gameObject.GetComponent<PlayerLevelManager>();
            if (collidedPlayer == null) return;
            if (gameManager.allPlayers.Count < 2) return;
            gameManager.allPlayers.ForEach(player =>
            {
                if (player == collidedPlayer) return;
                player.InvertControls();
                Notify(collidedPlayer);
                Debug.Log("POWER UP: Inverted controls");
                // consume power up
                Destroy(gameObject);
            });

        }
    }
}