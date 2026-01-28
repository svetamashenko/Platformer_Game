using System;
using System.Collections;
using UnityEngine;


namespace Assets.PixelCrew.Creatures.Mobs.Patrolling
{
    public class PointPatrol : Patrol
    {
        [SerializeField] private Transform[] _points;
        [SerializeField] private float _treshold = 1f;

        private Creature _creature;
        private int _destinationPoint;

        public void Awake()
        {
            _creature = GetComponent<Creature>();
        }
        public override IEnumerator DoPatrol()
        {
            while (enabled)
            {
                if (IsOnPoint())
                {
                    _destinationPoint = (int)Mathf.Repeat(_destinationPoint + 1, _points.Length);
                }

                var direction = _points[_destinationPoint].position - transform.position;
                direction.y = 0;
                _creature.SetDirection(direction.normalized);

                yield return null;
            }
        }

        private bool IsOnPoint()
        {
            return (_points[_destinationPoint].position - transform.position).magnitude < _treshold;
        }
    }
}