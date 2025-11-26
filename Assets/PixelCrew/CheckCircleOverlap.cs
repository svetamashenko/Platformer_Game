using Assets.PixelCrew.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PixelCrew
{
    public class CheckCircleOverlap : MonoBehaviour
    {
        [SerializeField] private float _radius = 1f;
        private readonly Collider2D[] _interactionResults = new Collider2D[5];
        public GameObject[] GetObjectsInRange()
        {
            var size = Physics2D.OverlapCircleNonAlloc(
                transform.position,
                _radius,
                _interactionResults);

            var overlaps = new List<GameObject>();
            for (int i = 0; i< size; i++)
            {
                overlaps.Add(_interactionResults[i].gameObject);
            }

            return overlaps.ToArray();
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = HandlesUtils.TransparentRed;
            Handles.DrawSolidDisc(transform.position, Vector3.forward, _radius);
        }
#endif
    }
}
