using UnityEngine;

namespace PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {
        public AudioSource sfxPlayer;
        protected GameManager gameManager;

        public readonly int requiredPlayers = 2;

        private void Awake()
        {
            gameManager = FindFirstObjectByType<GameManager>();
            sfxPlayer = GameObject.FindWithTag("SFX").GetComponent<AudioSource>();

            if (gameManager.allPlayers.Count < requiredPlayers)
            {
                Destroy(gameObject); // don't spawn power-up if not enough players
            }
        }
    }
}