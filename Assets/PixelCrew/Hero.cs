using Assets.PixelCrew.Model;
using PixelCrew.Components;
using UnityEditor.Animations;
using UnityEngine;

namespace PixelCrew
{
    public class Hero : MonoBehaviour
    {
        [SerializeField] private int _speed;
        [SerializeField] private int _jumpSpeed;
        [SerializeField] private int _damageJumpSpeed;
        [SerializeField] private int _damage;
        [SerializeField] private LayerCheck _groundCheck;

        [SerializeField] private float _interactionRadius;
        [SerializeField] private LayerMask _interactionLayer;

        [SerializeField] private AnimatorController _armed;
        [SerializeField] private AnimatorController _disarmed;

        [SerializeField] private SpawnComponent _footStepParticles;
        [SerializeField] private SpawnComponent _jumpParticles;
        [SerializeField] private SpawnComponent _fallParticles;

        [SerializeField] private ParticleSystem _hitParticles;
        private Rigidbody2D _rigidbody;
        private Vector2 _direction;

        [SerializeField] private CheckCircleOverlap _attackRange;
        private readonly Collider2D[] _interactionResults = new Collider2D[1];

        private Animator _animator;
        private static readonly int IsGroundedKey = Animator.StringToHash("is_grounded");
        private static readonly int IsRunningKey = Animator.StringToHash("is_running");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical_velocity");
        private static readonly int HitKey = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");


        private bool _isGrounded;
        private bool _allowDoubleJump;

        [SerializeField] private float _fallSpeed;
        [SerializeField] private float _minFallSpeed = 1f;
        private bool _hasSpawnedFallParticles;

        private GameSession _session;

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            var health = GetComponent<HealthComponent>();

            health.SetHealth(_session.Data.Hp);
            UpdateHeroWeapon();
        }
        private void UpdateHeroWeapon()
        {
            _animator.runtimeAnimatorController = _session.Data.IsArmed ? _armed : _disarmed;
        }

        private void FixedUpdate()
        {
            var xVelocity = _direction.x * _speed;
            var yVelocity = CalculateVelocity();
            _rigidbody.velocity = new Vector2(xVelocity, yVelocity);

            _animator.SetBool(IsGroundedKey, _isGrounded);
            _animator.SetFloat(VerticalVelocityKey, _rigidbody.velocity.y);
            _animator.SetBool(IsRunningKey, _direction.x != 0);

            UpdateSpriteDirection();

            _isGrounded = IsGrounded();
        }

        private void Update()
        {
            if (_isGrounded && !_hasSpawnedFallParticles && _fallSpeed > _minFallSpeed)
            {
                _fallParticles.Spawn();
                _hasSpawnedFallParticles = true;
            }
        }

        private float CalculateVelocity()
        {
            var yVelocity = _rigidbody.velocity.y;
            var isJumpPressing = _direction.y > 0;

            if (_isGrounded)
            {
                _allowDoubleJump = true;
                _fallSpeed = 0f;
                _hasSpawnedFallParticles = false;
            }
            else if (_rigidbody.velocity.y < -_fallSpeed)
            {
                _fallSpeed = -_rigidbody.velocity.y;
            }

            if (isJumpPressing)
            {
                yVelocity = CalculateJumpVelocity(yVelocity);
            }
            else if (_rigidbody.velocity.y > 0)
            {
                yVelocity *= 0.5f;
            }

            return yVelocity;
        }

        private float CalculateJumpVelocity(float yVelocity)
        {
            var isFalling = _rigidbody.velocity.y <= 0.001f;
            if (!isFalling) return yVelocity;

            if (_isGrounded)
            {
                yVelocity += _jumpSpeed;
                _jumpParticles.Spawn();
            }
            else if (_allowDoubleJump)
            {
                yVelocity = _jumpSpeed;
                _jumpParticles.Spawn();
                _allowDoubleJump = false;
            }
            return yVelocity;
        }


        private void UpdateSpriteDirection()
        {
            if (_direction.x > 0)
            {
                transform.localScale = Vector3.one;
            }
            else if (_direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        private bool IsGrounded()
        {
            return _groundCheck.IsTouchingLayer;
        }

        public static void SaySomething()
        {
            Debug.Log("Something...");
        }

        public void TakeDamage()
        {
            _animator.SetTrigger(HitKey);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _damageJumpSpeed);

            if (_session.Data.Coins > 0)
            {
                SpawnCoins();
            }
        }

        private void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(_session.Data.Coins, 5);
            _session.Data.Coins -= numCoinsToDispose;
            ShowBalance();

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);

            _hitParticles.gameObject.SetActive(true);
            _hitParticles.Play();
        }

        public void Interact()
        {
            var size = Physics2D.OverlapCircleNonAlloc(
                transform.position,
                _interactionRadius,
                _interactionResults,
                _interactionLayer
            );

            for (int i = 0; i < size; i++)
            {
                var interactable = _interactionResults[i].GetComponent<InteractableComponent>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }

        public void SpawnFootStepDust()
        {
            _footStepParticles.Spawn();
        }

        public void AddToBalance(int balance)
        {
            _session.Data.Coins += balance;
            ShowBalance();
        }

        private void ShowBalance()
        {
            Debug.Log($"Player has {_session.Data.Coins} currency.");
        }

        public void ResetBalance()
        {
            _session.Data.Coins = 0;
        }
        public void Attack()
        {
            if (!_session.Data.IsArmed)
            {
                return;
            }

            _animator.SetTrigger(AttackKey);
        }

        public void OnHealthChanged(int currentHealth)
        {
            _session.Data.Hp = currentHealth;
        }

        public void OnAttackApplying()
        {
            var gos = _attackRange.GetObjectsInRange();

            foreach (var go in gos)
            {
                var hp = go.GetComponent<HealthComponent>();
                if (hp != null && go.CompareTag("Enemy"))
                {
                    hp.ModifyHealth(-_damage);

                }
            }
        }
        public void ArmHero()
        {
            _session.Data.IsArmed = true;
            _animator.runtimeAnimatorController = _armed;
        }
    }
}
