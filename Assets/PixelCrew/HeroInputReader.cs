using UnityEngine;
using UnityEngine.InputSystem;

public class HeroInputReader : MonoBehaviour
{
    [SerializeField] private Hero _hero;

    private void OnMovement(InputValue context)
    {
        var direction = context.Get<Vector2>();
        _hero.SetDirection(direction);
    }

    private void OnSaySomething(InputValue context)
    {
        _hero.SaySomething();
    }
}
