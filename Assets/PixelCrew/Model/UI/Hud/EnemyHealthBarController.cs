using UnityEngine;
using PixelCrew.Components.Health;
using Assets.PixelCrew.Model.UI.Widgets;

namespace PixelCrew.Model.UI.Hud
{
    public class EnemyHealthBarController : MonoBehaviour
    {
        [SerializeField] private ProgressBarWidget _healthBar;
        private HealthComponent _healthComponent;

        public void SetHealthComponent(HealthComponent healthComponent)
        {
            _healthComponent = healthComponent;

            if (_healthBar == null)
            {
                _healthBar = GetComponent<ProgressBarWidget>();
                if (_healthBar == null)
                {
                    Debug.LogError("ProgressBarWidget не найден на объекте!");
                    return;
                }
            }

            _healthComponent.OnHealthChanged += OnHealthChanged;
            OnHealthChanged(_healthComponent.CurrentHealth);
        }

        private void OnHealthChanged(int currentHealth)
        {
            if (_healthBar == null) return;

            float fillAmount;
            if (_healthComponent.MaxHealth == 0)
                fillAmount = 0;
            else
                fillAmount = (float)currentHealth / _healthComponent.MaxHealth;
            _healthBar.SetProgress(Mathf.Clamp01(fillAmount));

            bool shouldBeVisible = currentHealth > 0;
            gameObject.SetActive(shouldBeVisible);
        }

        private void OnDestroy()
        {
            if (_healthComponent != null)
            {
                _healthComponent.OnHealthChanged -= OnHealthChanged;
            }
        }
    }
}
