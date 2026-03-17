using System;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components.Health
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private UnityEvent _onDamage;
        [SerializeField] private UnityEvent _onDie;
        [SerializeField] private UnityEvent _onHeal;
        [SerializeField] private HealthChangeEvent _onChange;

        [SerializeField] private int _maxHealth = 10;

        private bool _isInvincible;
        private int _pendingDamage;

        public int CurrentHealth => _health;
        public int MaxHealth => _maxHealth;
        public event Action<int> OnHealthChanged;

        public void ModifyHealth(int healthValue)
        {
            if (_health <= 0)
                return;

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

            if (_health <= 0)
            {
                OnHealthChanged?.Invoke(CurrentHealth);
                _onDie?.Invoke();
                OnHealthChanged?.Invoke(CurrentHealth);
                return;
            }
            else
            {
                if (_health >= _maxHealth)
                {
                    _health = _maxHealth;
                }
                _onChange?.Invoke(_health);
                OnHealthChanged?.Invoke(_health);

                if (damage < 0 && _health > 0)
                    _onDamage?.Invoke();
                else
                    _onHeal?.Invoke();
            }
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

        [Serializable]
        public class HealthChangeEvent : UnityEvent<int> { }

        public void SetHealth(int hp)
        {
            _health = hp;
        }
    }
}