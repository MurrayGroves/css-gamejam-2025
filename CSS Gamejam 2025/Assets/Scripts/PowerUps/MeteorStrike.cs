using Unity.VisualScripting;
using UnityEngine;

namespace PowerUps
{
    public class MeteorStrike : PowerUp
    {
        public GameObject meteorPrefab;
        public float spawnInterval = 0.5f;
        public AudioClip meteorSound;
        public PlayerLevelManager Player;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this.IsDestroyed()) return;
            base.Start();
            if (this.IsDestroyed()) return;
            var collidedPlayer = other.GetComponentInParent<PlayerLevelManager>();
            Debug.Log(collidedPlayer);
            if (collidedPlayer == null) return;
            Debug.Log("Summoning meteor strike!");
            Notify(collidedPlayer);

            // Hide the power-up visually but keep the GameObject alive for InvokeRepeating
            var renderer = GetComponent<SpriteRenderer>();
            if (renderer != null) renderer.enabled = false;
            var collider = GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;


            sfxPlayer.clip = meteorSound;
            sfxPlayer.Play();
            Player = collidedPlayer;
            InvokeRepeating(nameof(Strike), 0f, spawnInterval);
            Invoke(nameof(StopStrike), 5f);
        }

        public void Strike()
        {
            Camera cam = null;
            foreach (PlayerLevelManager player in gameManager.allPlayers)
            {
                if (player == Player) continue;
                cam = player.GetComponentInChildren<PlayerCam>()._cam;
            }

            float screenLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
            float screenRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
            float screenTop = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
            float randomX = Random.Range(screenLeft, screenRight);
            Vector3 spawnPos = new Vector3(randomX, screenTop + 1f, 0);
            GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        }

        public void StopStrike()
        {
            CancelInvoke(nameof(Strike));
            Destroy(gameObject); // Now destroy the entire GameObject after the effect completes
        }
    }
}