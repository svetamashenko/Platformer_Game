using Assets.PixelCrew.Creatures.Mobs.Patrolling;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Components.GoBased;
using PixelCrew.Model;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.PixelCrew.Creatures.Mobs
{
    public class FlyingTrapAI : MonoBehaviour
    {
        [SerializeField] private LayerCheck _vision;
        [SerializeField] private float _flySpeed = 5f;

        [SerializeField] private PlatformPatrol platformPatrol;
        [SerializeField] private LifeTimeControllerComponent lifeTimeController;

        [SerializeField] private UnityEvent _actionsAfterLanding;

        private GameObject _hero;
        private Transform _heroTransform;
        private Vector3 _targetPos;
        private bool _isDead = false;
        private Animator _animator;
        private Collider2D _collider2D;

        private Coroutine _current;

        private GameSession _session;
        private int SwordCount => _session.Data.Inventory.Count("Sword");

        private void Awake()
        {
            _session = FindObjectOfType<GameSession>();
            _animator = GetComponent<Animator>();
            _collider2D = GetComponent<Collider2D>();
        }

        private void Start()
        {
            StartState(platformPatrol.DoPatrol());
        }

        private void Update()
        {
            if (_hero != null)
            {
                if (SwordCount == 0)
                {
                    _targetPos = new Vector3(_heroTransform.position.x, _heroTransform.position.y + 0.5f, transform.position.z);
                }
                else
                {
                    _targetPos = new Vector3(
                          _heroTransform.position.x + (_heroTransform.localScale.x >= 0 ? -0.22f : 0.22f),
                          _heroTransform.position.y + 0.5f,
                          transform.position.z
                    );
                }
            }
        }

        public void OnHeroInVision(GameObject hero)
        {
            if (!_isDead)
            {
                _animator.SetBool("heroFound", true);
                _hero = hero;
                _heroTransform = hero.transform;
                _targetPos = new Vector3(_heroTransform.position.x, _heroTransform.position.y + 0.5f, transform.position.z);
                StartState(FlyAndEngage());
                _collider2D.isTrigger = true;
            }
        }

        private IEnumerator FlyAndEngage()
        {
            while (transform.position != _targetPos)
            {
                MoveTowardsHero();
                yield return null;
            }
            _actionsAfterLanding?.Invoke();

            lifeTimeController.StartTimer();
            StartState(FollowHero());
        }

        private IEnumerator FollowHero()
        {
            while (_hero != null)
            {
                transform.position = _targetPos;
                yield return null;
            }
        }

        private void MoveTowardsHero()
        {
            if (_hero != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetPos, Time.deltaTime * _flySpeed);
            }
        }

        private bool IsDead()
        {
            return _isDead || !gameObject.activeSelf;
        }

        private void StartState(IEnumerator coroutine)
        {
            if (_current != null)
            {
                StopCoroutine(_current);
            }
            _current = StartCoroutine(coroutine);
        }
    }
}