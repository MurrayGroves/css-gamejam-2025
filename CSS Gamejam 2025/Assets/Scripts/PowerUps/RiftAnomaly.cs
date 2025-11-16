using UnityEngine;
namespace PowerUps
{
    public class RiftAnomaly : PowerUp
    {
        private AudioSource sfxPlayer;
        public AudioClip riftSound;
        private void Awake()
        {
            sfxPlayer = GameObject.FindWithTag("SFX").GetComponent<AudioSource>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Swapping positions");
            sfxPlayer.clip = riftSound;
            sfxPlayer.Play();
            var playerOne = Player.gameManager.allPlayers[0];
            var playerTwo = Player.gameManager.allPlayers[1];
            var oldPosPlayerOne = playerOne.transform.position;
            playerOne.Teleport(playerTwo.transform.position);
            playerTwo.Teleport(oldPosPlayerOne);
            Destroy(this);
        }
    }
}