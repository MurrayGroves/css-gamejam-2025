using UnityEngine;

namespace PowerUps
{
    public class StealSpeed : PowerUp
    {
        private const int Speed = 100;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Stealing speed");

            PlayerLevelManager.AddSpeed(Speed);
            PlayerLevelManager.gameManager.allPlayers.ForEach(player =>
            {
                if (player != PlayerLevelManager)
                {
                    player.ReduceSpeed(Speed);
                }
            });
        }
    }
}