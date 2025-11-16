using UnityEngine;
namespace PowerUps
{
    public class MeteorStrike : PowerUp
    {
        public GameObject meteorPrefab;
        public float spawnInterval = 0.5f;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            InvokeRepeating(nameof(Strike), 0f, spawnInterval);
            Invoke(nameof(StopStrike), 5f);
        }

        public void Strike()
        {
            Vector3 spawnPos = new Vector3();
            GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        }
        public void StopStrike()
        {
            CancelInvoke(nameof(Strike));
            Destroy(this);
        }
    }
}