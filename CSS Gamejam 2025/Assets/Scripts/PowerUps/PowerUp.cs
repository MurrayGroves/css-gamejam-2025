using UnityEngine;

namespace PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {
        public AudioSource sfxPlayer;
        protected GameManager gameManager;
        
        private void Awake()
        {
            gameManager = FindFirstObjectByType<GameManager>();
            sfxPlayer = GameObject.FindWithTag("SFX").GetComponent<AudioSource>();
        }
    }
}