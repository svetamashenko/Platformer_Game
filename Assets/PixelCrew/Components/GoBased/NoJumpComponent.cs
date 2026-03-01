using UnityEngine;

namespace PixelCrew.Components.GoBased
{
    public class NoJumpComponent : MonoBehaviour
    {
        //[SerializeField] private float slowFactor = 0.1f;

        private GameObject _hero;
        private Rigidbody2D _heroRb;
        private Vector2 initialVelocity;

        public void Activate(GameObject hero)
        {
            _hero = hero;
            _heroRb = _hero.GetComponent<Rigidbody2D>();
            if (_heroRb != null)
            {
                initialVelocity = _heroRb.velocity;
            }
        }

        public void Deactivate()
        {
            if (_heroRb != null)
            {
                _heroRb.velocity = initialVelocity;
            }
            _hero = null;
        }

        private void FixedUpdate()
        {
            if (_heroRb != null)
            {
                _heroRb.velocity = new Vector2(_heroRb.velocity.x, 0.1f);
            }
        }
    }
}