using UnityEngine;

namespace PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {
        public PlayerLevelManager Player;
        
        public void SetLevelManager(PlayerLevelManager playerLevelManager)
        {
            Player = playerLevelManager;
        }
    }
}