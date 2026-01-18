using System;
using UnityEngine;

namespace Assets.PixelCrew.Utils
{
    [Serializable]
    public class Cooldown
    {
        [SerializeField] private float _value;

        private float _timesUp;

        public void Reset()
        {
            _timesUp = Time.time + _value;
        }

        public bool IsReady => _timesUp <= Time.time;

        public void ResetWithDelayPercent(int percent)
        {
            float delay = _value * (percent / 100f);
            _timesUp = Time.time + delay;
        }
    }
}
