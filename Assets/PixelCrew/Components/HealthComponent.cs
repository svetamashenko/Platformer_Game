using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private int _health;
    [SerializeField] private UnityEvent _onDamage;
    [SerializeField] private UnityEvent _onDie;
    [SerializeField] private UnityEvent _onHeal;
    [SerializeField] private int _maxHealth = 9;

    public void ApplyDamage(int damageValue)
    {
        _health -= damageValue;
        _onDamage?.Invoke();
        if (_health <= 0)
        {
            _onDie?.Invoke();
        }
    }

    public void ApplyHeal(int healValue)
    {
        _health += healValue;
        _onHeal?.Invoke();
        if (_health >= _maxHealth)
        {
            _health = _maxHealth;
        }
    }
}
