using PixelCrew.Components.GoBased;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCrew.Creatures.Hero
{
    public class HeroInputReader : MonoBehaviour
    {
        [SerializeField] private Hero _hero;
        [SerializeField] private PlatformGeneratorComponent _platformGenerator;
        [SerializeField] private int _healingOfPotion = 5;

        private float _pressStartTime;
        private const float _longPressThreshold = 1f;
        protected PlaySoundsComponent Sounds;

        public void OnThrow(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
            {
                _pressStartTime = Time.unscaledTime;
            }

            else if (context.canceled)
            {
                float pressDuration = Time.unscaledTime - _pressStartTime;

                if (pressDuration >= _longPressThreshold)
                {
                    if (_hero != null)
                    {
                        _hero.ThrowMultiple(3);
                    }
                }
                else
                {
                    _hero.Throw();
                }
            }
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            var direction = context.ReadValue<Vector2>();
            _hero.SetDirection(direction);
        }

        public void OnSaySomething(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                Hero.SaySomething();
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                _hero.Interact();
            }
        }

        public void OnGeneratePlatform(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                _platformGenerator.GeneratePlatform(_hero.transform);
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                _hero.Attack();
            }
        }
        public void OnUsePotion(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                _hero.ApplyHealing(_healingOfPotion);
            }
        }
    }
}
