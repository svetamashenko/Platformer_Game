using UnityEngine;

namespace Assets.PixelCrew.Movements
{
    public class CircleLevitationComponent : MonoBehaviour
    {
        [SerializeField] private float _radius = 0f;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private bool _randomizeStartAngle = true;

        private Rigidbody2D _rigidbody;
        private Vector2 _centerPosition;
        private float _startAngle;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _centerPosition = (Vector2)_rigidbody.position;

            if (_randomizeStartAngle)
                _startAngle = Random.Range(0f, 2f * Mathf.PI);
            else
                _startAngle = 0f;
        }

        private void Update()
        {
            float angle = _startAngle + Time.time * _speed;
            Vector2 offset = new Vector2(
                Mathf.Cos(angle) * _radius,
                Mathf.Sin(angle) * _radius
            );

            Vector2 newPosition = _centerPosition + offset;
            _rigidbody.MovePosition(newPosition);
        }
    }
}
