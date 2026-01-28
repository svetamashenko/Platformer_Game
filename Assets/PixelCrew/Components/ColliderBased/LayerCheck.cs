using UnityEngine;

namespace PixelCrew.Components.ColliderBased
{
    public class LayerCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayer;
        private Collider2D _collider;

        public bool IsTouchingLayer;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            IsTouchingLayer = _collider.IsTouchingLayers(_groundLayer);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            IsTouchingLayer = _collider.IsTouchingLayers(_groundLayer);
        }
        public LayerMask GetGroundLayer() => _groundLayer;

    }
}