using System;
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
        }

        public void Start()
        {
            // Check if it's a child of a tilemap or grid
            var tilemapParent = GetComponentInParent<UnityEngine.Tilemaps.Tilemap>();
            
            if (tilemapParent != null)
            {
                Debug.Log($"  Parent Tilemap: {tilemapParent.name} at position {tilemapParent.transform.position}");
            }


            // Only spawn power-ups in chunks that are far enough into the game
            // Find the actual chunk prefab (ChunkPrefabPlatform1, ChunkPrefabPlatform3, etc.)
            Transform chunkTransform = null;
            Transform current = transform;
            while (current != null)
            {
                if (current.name.Contains("Chunk") && current.name.Contains("Prefab"))
                {
                    chunkTransform = current;
                    break;
                }
                current = current.parent;
            }

            if (chunkTransform != null)
            {
                Debug.Log($"[PowerUp] Found chunk: {chunkTransform.name} at position {chunkTransform.position}");
                if (chunkTransform.position.x < 30)
                {
                    Debug.Log($"[PowerUp] Destroying {GetType().Name} - Chunk {chunkTransform.name} X position {chunkTransform.position.x} < 30");
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                Debug.LogWarning($"[PowerUp] Could not find chunk prefab ancestor for {GetType().Name}");
            }
            
            // if (gameManager.allPlayers.Count < requiredPlayers)
            // {
            //     Debug.Log(
            //         $"[PowerUp] Destroying {GetType().Name} - not enough players ({gameManager.allPlayers.Count}/{requiredPlayers})");
            //     Destroy(gameObject); // don't spawn power-up if not enough players
            // }
        }

        protected void Notify(PlayerLevelManager player)
        {
            if (gameManager != null)
            {
                gameManager.NotifyPowerUpActivated(GetType().Name.Replace(" ", ""), player);
            }
        }
    }
}