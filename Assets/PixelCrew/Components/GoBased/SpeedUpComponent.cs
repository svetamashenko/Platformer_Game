using Assets.PixelCrew.Creatures;
using UnityEngine;

namespace PixelCrew.Components.GoBased
{
    public class SpeedUpComponent : MonoBehaviour
    {
        [SerializeField] private float _speedMultiplier = 2f;
        [SerializeField] private float _duration = 5f;

        private Creature _creature;
        private int _originalSpeed;
        private float _timer;

        public void Activate(Creature creature)
        {
            _creature = creature;
            _originalSpeed = creature.GetSpeed();
            creature.SetSpeed(Mathf.RoundToInt(_originalSpeed * _speedMultiplier));
            _timer = _duration;
        }

        private void Update()
        {
            if (_creature == null) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                Deactivate();
            }
        }

        public void Deactivate()
        {
            if (_creature != null)
            {
                _creature.SetSpeed(_originalSpeed);
                _creature = null;
            }
            Destroy(gameObject);
        }
    }
}