namespace PowerUps
{
    public class StealSpeedReceiver : PowerUpReceiver
    {
        private float _originalValue;

        private PlayerLevelManager _player;
        protected override float Duration => 5.0f;

        private void Awake()
        {
            _player = GetComponent<PlayerLevelManager>();
        }

        protected override void ResetEffects()
        {
            _player.SetSpeed(_originalValue);
        }

        public void ApplyEffects(float multiplier)
        {
            if (!SetActive()) _originalValue = _player.GetSpeed();

            _player.SetSpeed(_originalValue * multiplier);
        }
    }
}