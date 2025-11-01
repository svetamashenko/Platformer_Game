using UnityEngine;
using UnityEngine.InputSystem;

namespace PixelCrew
{
    public class HeroInputReader : MonoBehaviour
    {
        [SerializeField] private Hero _hero;

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
    }
}
