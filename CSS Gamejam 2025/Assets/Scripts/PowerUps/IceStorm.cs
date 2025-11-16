using UnityEngine;

namespace PowerUps
{
    public class IceStorm : PowerUp
    {
        private const int Speed = 15;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Player.gameManager.allPlayers.ForEach(player => player.IncreaseSpeed(Speed));
            Debug.Log("Ice storm");
        }
    }
}