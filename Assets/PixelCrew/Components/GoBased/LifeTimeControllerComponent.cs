using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using PixelCrew.Components.Animations;

namespace PixelCrew.Components.GoBased
{
    public class LifeTimeControllerComponent : MonoBehaviour
    {
        [SerializeField] private float activeDuration = 3f;
        [SerializeField] private UnityEvent onDisappearanceStarted;

        private SpriteAnimation _spriteAnim;
        private bool _isDisappearing = false;

        private void Awake()
        {
            _spriteAnim = GetComponent<SpriteAnimation>();
        }

        private void Start()
        {
            StartCoroutine(LifetimeCountdown());
        }

        public void ForceDisappear()
        {
            if (_isDisappearing) return;

            _isDisappearing = _spriteAnim.AddDisappearing;
            onDisappearanceStarted?.Invoke();

            if (_spriteAnim != null)
            {
                _spriteAnim.StartDisappearing();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator LifetimeCountdown()
        {
            yield return new WaitForSeconds(activeDuration);
            ForceDisappear();
        }
    }
}
