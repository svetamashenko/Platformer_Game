using UnityEngine;

namespace PixelCrew.Components.Health
{
    public class ModifyHealthComponent : MonoBehaviour
    {
        [SerializeField] private int _healthModifier = 0;

        private HealthComponent GetHealth(GameObject target)
            => target.GetComponent<HealthComponent>();

        public void Apply(GameObject target)
        {
            var health = GetHealth(target);
            if (health != null)
            {
                health.ModifyHealth(_healthModifier);
            }
        }

        public void OnAnimationEnd() => Destroy(gameObject);
    }
}