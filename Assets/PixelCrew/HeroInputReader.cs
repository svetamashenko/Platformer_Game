using PixelCrew.Components;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCrew
{
    public class HeroInputReader : MonoBehaviour
    {
        [SerializeField] private Hero _hero;
        [SerializeField] private PlatformGeneratorComponent _platformGenerator;

        private void OnMovement(InputValue context)
        {
            var direction = context.Get<Vector2>();
            _hero.SetDirection(direction);
        }

        private void OnSaySomething()
        {
            Hero.SaySomething();
        }

        private void OnInteract()
        {
            _hero.Interact();
        }

        private void OnGeneratePlatform()
        {
            _platformGenerator.GeneratePlatform(_hero.transform);
        }
    }
}
