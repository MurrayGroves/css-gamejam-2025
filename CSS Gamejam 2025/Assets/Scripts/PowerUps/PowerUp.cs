using UnityEngine;

namespace PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {
        public PlayerLevelManager Player;
        public AudioSource sfxPlayer;
        
        private void Awake()
        {
            sfxPlayer = GameObject.FindWithTag("SFX").GetComponent<AudioSource>();
        }
        public void SetLevelManager(PlayerLevelManager playerLevelManager)
        {
            Player = playerLevelManager;
        }
    }
}