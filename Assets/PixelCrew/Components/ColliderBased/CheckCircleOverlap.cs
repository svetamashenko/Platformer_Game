using PixelCrew.Utils;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components.ColliderBased
{
    public class CheckCircleOverlap : MonoBehaviour
    {
        [SerializeField] private float _radius = 1f;
        [SerializeField] private LayerMask _mask;
        [SerializeField] private string[] _tags;
        [SerializeField] private OnOverlapEvent _onOverlap;
        private readonly Collider2D[] _interactionResults = new Collider2D[10];

        public void Check()
        {
            var size = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            _radius,
            _interactionResults,
            _mask);

            for (int i = 0; i < size; i++)
            {
                var overlapResult = _interactionResults[i];
                var isInTags = _tags.Any(tag => overlapResult.CompareTag(tag));
                if (isInTags)
                {
                    _onOverlap?.Invoke(overlapResult.gameObject);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = HandlesUtils.TransparentRed;
            Handles.DrawSolidDisc(transform.position, Vector3.forward, _radius);
        }
#endif

        [Serializable]
        private class OnOverlapEvent : UnityEvent<GameObject>
        {
        }
    }
}
