using UnityEngine;

namespace PixelCrew.Components
{
    public class EnterTriggerComponent : MonoBehaviour
    {
        [SerializeField] private string _tag;
        [SerializeField] private LayerMask _layer = ~0;
        public EnterEvent _action;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!(((1 << other.gameObject.layer) & _layer) != 0)) return;
            if (!string.IsNullOrEmpty(_tag) && !other.gameObject.CompareTag(_tag)) return;

            _action?.Invoke(other.gameObject);
        }
    }
}