using UnityEngine;

namespace PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {
        protected GameManager gameManager;
        public PlayerLevelManager Player;
        public AudioSource sfxPlayer;

        public void SetLevelManager(PlayerLevelManager player)
        {
            Player = player;
        }
        
        private void Awake()
        {
            gameManager = FindFirstObjectByType<GameManager>();
            sfxPlayer = GameObject.FindWithTag("SFX").GetComponent<AudioSource>();
        }
    }
}