using PixelCrew;
using System.Collections;
using UnityEngine;

namespace Assets.PixelCrew.Creatures
{
    public class PlatformPatrol : Patrol
    {
        [SerializeField] private LayerCheck _groundCheck;
        [SerializeField] private float _lookaheadDistance = 0.5f;
        [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.2f, 0.1f);
        [SerializeField] private float _obstacleCheckDistance = 0.4f;

        private Creature _creature;
        private bool _isFacingRight = true;


        private void Awake()
        {
            _creature = GetComponent<Creature>();
            _obstacleCheckDistance += _lookaheadDistance;
        }


        public override IEnumerator DoPatrol()
        {
            while (enabled)
            {
                bool needsToTurn = CheckNeedToTurn();
                if (needsToTurn)
                {
                    _isFacingRight = !_isFacingRight;
                }

                float directionX = _isFacingRight ? 1f : -1f;
                _creature.SetDirection(new Vector2(directionX, 0f));

                yield return null;
            }
        }

        private bool CheckNeedToTurn()
        {
            bool isAtEdge = !CheckGroundInFront();
            bool hasObstacle = CheckObstacleInFront();

            return isAtEdge || hasObstacle;
        }

        private bool CheckGroundInFront()
        {
            Vector2 startPos = (Vector2)transform.position;
            if (_isFacingRight)
                startPos += Vector2.right * _lookaheadDistance;
            else
                startPos += Vector2.left * _lookaheadDistance;


            Vector2 checkPos = startPos + Vector2.down * 0.1f;


            Collider2D hit = Physics2D.OverlapBox(
                checkPos,
                _groundCheckSize,
                0f,
                _groundCheck.GetGroundLayer()
            );

            return hit != null;
        }

        private bool CheckObstacleInFront()
        {
            Vector2 startPos = (Vector2)transform.position;
            startPos += Vector2.up;

            Vector2 direction = _isFacingRight ? Vector2.right : Vector2.left;


            RaycastHit2D hit = Physics2D.Raycast(
                startPos,
                direction,
                _obstacleCheckDistance,
                _groundCheck.GetGroundLayer()
            );

            return hit.collider != null;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            var startPos = transform.position;
            Vector2 groundCheckPos, obstacleCheckPos;

            if (_isFacingRight)
            {
                groundCheckPos = (Vector2)startPos + Vector2.right * _lookaheadDistance;
                obstacleCheckPos = (Vector2)startPos + Vector2.right * _obstacleCheckDistance;
            }
            else
            {
                groundCheckPos = (Vector2)startPos + Vector2.left * _lookaheadDistance;
                obstacleCheckPos = (Vector2)startPos + Vector2.left * _obstacleCheckDistance;
            }

            groundCheckPos += Vector2.down * 0.1f;
            obstacleCheckPos += Vector2.up * 0.5f;

            Gizmos.color = CheckGroundInFront() ? Color.green : Color.red;
            Gizmos.DrawWireCube(groundCheckPos, _groundCheckSize);

            Gizmos.color = Color.blue; Gizmos.DrawLine(
                 startPos + new Vector3(0, 0.5f, 0),
                 obstacleCheckPos
             );
        }
    }
}
