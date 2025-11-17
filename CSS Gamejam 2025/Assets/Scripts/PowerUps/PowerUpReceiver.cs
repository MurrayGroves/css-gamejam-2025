using System.Collections;
using UnityEngine;

namespace PowerUps
{
    public abstract class PowerUpReceiver : MonoBehaviour
    {
        private Coroutine _timer;
        private float _waitingSince;

        protected abstract float Duration { get; }

        protected bool SetActive()
        {
            var duration = Duration;

            var active = _timer != null;
            if (active)
            {
                duration += Duration - (Time.time - _waitingSince);
                StopCoroutine(_timer);
            }

            _timer = StartCoroutine(Timer(duration));

            return active;
        }

        private IEnumerator Timer(float duration)
        {
            yield return new WaitForSeconds(duration);
            ResetEffects();
        }

        protected abstract void ResetEffects();
    }
}