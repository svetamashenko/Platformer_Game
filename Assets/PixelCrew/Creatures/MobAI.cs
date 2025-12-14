using Assets.PixelCrew.Components;
using PixelCrew;
using System.Collections;
using UnityEngine;

namespace Assets.PixelCrew.Creatures
{
    public class MobAI : MonoBehaviour
    {
        [SerializeField] private LayerCheck _vision;
        [SerializeField] private LayerCheck _canAttack;

        [SerializeField] private float _alarmDelay = 0.5f;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _missCooldown = 0.5f;
        private Coroutine _current;
        private GameObject _target;

        private static readonly int IsDeadKey = Animator.StringToHash("is_dead");

        private SpawnListComponent _particles;
        private Creature _creature;
        private Animator _animator;
        private bool _isDead;
        private Patrol _patrol;

        private void Awake()
        {
            _particles = GetComponent<SpawnListComponent>();
            _creature = GetComponent<Creature>();
            _animator = GetComponent<Animator>();
            _patrol = GetComponent<Patrol>();
        }

        private void Start()
        {
            StartState(_patrol.DoPatrol());
        }

        public void OnHeroInVision(GameObject go)
        {
            if (!_isDead)
            {
                _target = go;
                StartState(AgroToHero());
            }
        }

        private IEnumerator AgroToHero()
        {
            _particles.Spawn("Exclamation");
            yield return new WaitForSeconds(_alarmDelay);
            StartState(GoToHero());
        }

        private IEnumerator GoToHero()
        {
            while (_vision.IsTouchingLayer)
            {
                if (_canAttack.IsTouchingLayer)
                {
                    StartState(Attack());
                    yield break;
                }
                else
                {
                    SetDirectionToTarget();
                }
                yield return null;
            }
            _particles.Spawn("Miss");
            yield return new WaitForSeconds(_missCooldown);
            StartState(_patrol.DoPatrol());
        }


        private IEnumerator Attack()
        {
            while (_canAttack.IsTouchingLayer)
            {
                _creature.Attack();
                yield return new WaitForSeconds(_attackCooldown);
            }

            StartState(GoToHero());
        }

        private void SetDirectionToTarget()
        {
            if (_isDead) return;

            var direction = _target.transform.position - transform.position;
            direction.y = 0;
            _creature.SetDirection(direction.normalized);
        }

        private void StartState(IEnumerator coroutine)
        {
            _creature.SetDirection(Vector2.zero);
            if (_current != null)
            {
                StopCoroutine(_current);
            }
            _current = StartCoroutine(coroutine);
        }

        public void OnDie()
        {
            _isDead = true;

            if (_animator != null)
            {
                _animator.SetBool(IsDeadKey, true);
                _animator.Play("die");
            }

            if (_current != null)
            {
                StopCoroutine(_current);
                _current = null;
            }

            if (_patrol != null)
            {
                _patrol.enabled = false;
            }

            _target = null;
        }
    }
}
