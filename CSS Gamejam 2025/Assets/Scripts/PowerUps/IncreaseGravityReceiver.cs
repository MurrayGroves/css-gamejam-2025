using UnityEngine;

namespace PowerUps
{
    public class IncreaseGravityReceiver : PowerUpReceiver
    {
        private float _originalValue;

        private Rigidbody2D _rb;
        protected override float Duration => 5;

        private void Awake()
        {
            _rb = GetComponent<PlayerLevelManager>().MovementController.GetComponent<Rigidbody2D>();
        }

        protected override void ResetEffects()
        {
            _rb.gravityScale = _originalValue;
        }

        public void ApplyEffects(float multiplier)
        {
            if (SetActive()) return;

            _originalValue = _rb.gravityScale;
            _rb.gravityScale = _originalValue * multiplier;
            Debug.Log($"Increased gravity from {_originalValue} to {_rb.gravityScale}, multiplier is {multiplier}");
        }
    }
}