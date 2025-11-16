using UnityEngine;

namespace PowerUps
{
    public class StealSpeed : PowerUp
    {
        private const int Speed = 5;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Stealing speed");

            Player.IncreaseSpeed(Speed);
            Player.gameManager.allPlayers.ForEach(player =>
            {
                if (player != Player)
                {
                    player.ReduceSpeed(Speed);
                }
            });
        }
    }
}