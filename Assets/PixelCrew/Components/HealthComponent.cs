using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private UnityEvent _onDamage;
        [SerializeField] private UnityEvent _onDie;
        [SerializeField] private UnityEvent _onHeal;
        [SerializeField] private int _maxHealth = 9;

        private bool _isInvincible;
        private int _pendingDamage;

        public void ModifyHealth(int healthValue)
        {
            if (healthValue < 0 && _isInvincible)
            {
                _pendingDamage += healthValue;
                return;
            }

            ApplyDamage(healthValue);
        }

        private void ApplyDamage(int damage)
        {
            _health += damage;

            if (damage < 0 && _health > 0)
                _onDamage?.Invoke();
            else
                _onHeal?.Invoke();

            if (_health <= 0)
            {
                _onDie?.Invoke();
                return;
            }
            if (_health >= _maxHealth)
                _health = _maxHealth;
        }

        public void OnHitAnimationEnd()
        {
            _isInvincible = false;

            if (_pendingDamage != 0)
            {
                ApplyDamage(_pendingDamage);
                _pendingDamage = 0;
            }
        }
    }
}
