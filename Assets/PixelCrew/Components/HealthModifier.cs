using UnityEngine;

namespace PixelCrew
{
    public class HealthModifier : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _animationKey;

        [SerializeField] private int _damageAmount = 0;
        [SerializeField] private int _healAmount = 0;

        private HealthComponent GetHealth(GameObject target)
            => target.GetComponent<HealthComponent>();

        public void DealDamage(GameObject target)
        {
            var health = GetHealth(target);
            if (health != null)
            {
                health.ApplyDamage(_damageAmount);
            }
        }

        public void RestoreHealth(GameObject target)
        {
            var health = GetHealth(target);
            if (health != null)
            {
                health.ApplyHeal(_healAmount);
                _animator.SetBool(_animationKey, true);
            }
        }

        public void OnAnimationEnd() => Destroy(gameObject);
    }
}