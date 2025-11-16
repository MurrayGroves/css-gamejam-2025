using UnityEngine;

namespace PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {
        public PlayerLevelManager Player;
        
        protected abstract void OnTriggerEnter2D(Collider2D other);
        
        public void SetLevelManager(PlayerLevelManager playerLevelManager)
        {
            Player = playerLevelManager;
        }
    }
}