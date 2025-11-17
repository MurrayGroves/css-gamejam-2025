namespace PowerUps
{
    public class InvertControlsReceiver : PowerUpReceiver
    {
        private PlayerLevelManager _player;
        protected override float Duration => 5.0f;

        private void Awake()
        {
            _player = GetComponent<PlayerLevelManager>();
        }

        protected override void ResetEffects()
        {
            _player.RevertControls();
        }

        public void ApplyEffects()
        {
            if (SetActive()) return;

            _player.InvertControls();
        }
    }
}