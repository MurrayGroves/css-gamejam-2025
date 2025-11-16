using UnityEngine;
namespace PowerUps
{
    public class MeteorStrike : PowerUp
    {
        public GameObject meteorPrefab;
        public float spawnInterval = 0.5f;
        public AudioClip meteorSound;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var collidedPlayer = other.GetComponentInParent<PlayerLevelManager>();
            if (!collidedPlayer) return;
            Debug.Log("Summoning meteor strike!");
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            sfxPlayer.clip = meteorSound;
            sfxPlayer.Play();
            InvokeRepeating(nameof(Strike), 0f, spawnInterval);
            Invoke(nameof(StopStrike), 5f);
        }

        public void Strike()
        {
            float screenLeft = 0;
            float screenRight = 100;
            float screenTop = -300;
            float randomX = Random.Range(screenLeft, screenRight);
            Vector3 spawnPos = new Vector3(randomX, screenTop + 1f, 0);
            GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        }
        
        public void StopStrike()
        {
            CancelInvoke(nameof(Strike));
            Destroy(this);
        }
    }
}