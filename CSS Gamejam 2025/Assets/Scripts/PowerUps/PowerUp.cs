using UnityEngine;

namespace PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {
        [SerializeField] protected PlayerLevelManager PlayerLevelManager;
        
        public void SetLevelManager(PlayerLevelManager playerLevelManager)
        {
            PlayerLevelManager = playerLevelManager;
        }
    }
}