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
        [SerializeField] private UnityEvent beforeDestroy;
        [SerializeField] private bool onStart = false;

        private SpriteAnimation _spriteAnim;
        private bool _isDisappearing = false;

        private void Awake()
        {
            _spriteAnim = GetComponent<SpriteAnimation>();
        }

        private void Start()
        {
            if (onStart)
            {
                StartCoroutine(LifetimeCountdown());
            }
        }

        public void ForceDisappear()
        {
            if (_isDisappearing) return;

            if (_spriteAnim != null)
            {
                _isDisappearing = true;
                onDisappearanceStarted?.Invoke();
                _spriteAnim.StartDisappearing();
            }
            else
            {
                BeforeDestroy();
                Destroy(gameObject);
            }
        }

        private IEnumerator LifetimeCountdown()
        {
            yield return new WaitForSeconds(activeDuration);
            ForceDisappear();
        }

        public void StartTimer()
        {
            StartCoroutine(LifetimeCountdown());
        }

        private void BeforeDestroy()
        {
            beforeDestroy?.Invoke();
        }

        private void OnDestroy()
        {
            BeforeDestroy();
        }
    }
}