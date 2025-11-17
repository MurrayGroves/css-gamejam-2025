using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace PowerUps
{
    public class MeteorStrike : PowerUp
    {
        public GameObject meteorPrefab;
        public float spawnInterval = 0.5f;
        public AudioClip meteorSound;

        private List<PlayerLevelManager> _otherPlayers;

        protected override void OnPickup(PlayerLevelManager pickupPlayer, IEnumerable<PlayerLevelManager> otherPlayers)
        {
            if (this.IsDestroyed()) return;
            Start();
            if (this.IsDestroyed()) return;
            Debug.Log("Summoning meteor strike!");
            Notify(pickupPlayer);

            // Hide the power-up visually but keep the GameObject alive for InvokeRepeating
            var renderer = GetComponent<SpriteRenderer>();
            if (renderer != null) renderer.enabled = false;
            var collider = GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;


            sfxPlayer.clip = meteorSound;
            sfxPlayer.Play();
            _otherPlayers = otherPlayers.ToList();
            InvokeRepeating(nameof(Strike), 0f, spawnInterval);
            Invoke(nameof(StopStrike), 5f);
        }

        public void Strike()
        {
            Camera cam = null;
            foreach (var player in _otherPlayers) cam = player.GetComponentInChildren<PlayerCam>()._cam;

            var screenLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
            var screenRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
            var screenTop = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
            var randomX = Random.Range(screenLeft, screenRight);
            var spawnPos = new Vector3(randomX, screenTop + 1f, 0);
            var meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        }

        public void StopStrike()
        {
            CancelInvoke(nameof(Strike));
            Destroy(gameObject); // Now destroy the entire GameObject after the effect completes
        }
    }
}