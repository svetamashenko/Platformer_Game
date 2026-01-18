using UnityEngine;

namespace Assets.PixelCrew.Movements
{
    public class VerticalLevitationComponent : MonoBehaviour
    {
        [SerializeField] private float _frequency = 6f;
        [SerializeField] private float _amplitude = 0.1f;
        [SerializeField] private bool _randomize = true;

        private float _originalY;
        private Rigidbody2D _rigidbody;
        private float _seed;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _originalY = _rigidbody.position.y;
            if (_randomize)
                _seed = Random.value * Mathf.PI * 2;
        }

        private void Update()
        {
            var pos = _rigidbody.position;
            pos.y = _originalY + Mathf.Sin(_seed + Time.time * _frequency) * _amplitude;
            _rigidbody.MovePosition(pos);
        }

    }
}