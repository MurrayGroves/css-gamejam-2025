using UnityEngine;

namespace PowerUps
{
    public class StealSpeed : PowerUp
    {
        private const int Speed = 10;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Stealing speed");

            PlayerLevelManager.AddSpeed(Speed);
            PlayerLevelManager.gameManager.allPlayers.ForEach(player => player.ReduceSpeed(Speed));
        }
    }
}