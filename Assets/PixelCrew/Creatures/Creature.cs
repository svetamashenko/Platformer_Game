using Assets.PixelCrew.Components.GoBased;
using PixelCrew.Components.ColliderBased;
using UnityEngine;

namespace Assets.PixelCrew.Creatures
{
    public class Creature : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private bool _invertScale;
        [SerializeField] private int _speed;
        [SerializeField] protected int JumpSpeed;
        [SerializeField] private int _damageJumpSpeed;
        [SerializeField] private int _damage;

        [Header("Checkers")]
        [SerializeField] private LayerCheck _groundCheck;
        [SerializeField] private CheckCircleOverlap _attackRange;
        [SerializeField] protected SpawnListComponent Particles;
        [SerializeField] private float _minFallSpeed = 11.7f;

        private float _fallSpeed;
        protected Rigidbody2D Rigidbody;
        protected Vector2 Direction;
        protected PlaySoundsComponent Sounds;
        protected bool IsGrounded;

        protected Animator Animator;
        private static readonly int IsGroundedKey = Animator.StringToHash("is_grounded");
        private static readonly int IsRunningKey = Animator.StringToHash("is_running");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical_velocity");
        private static readonly int HitKey = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");
        protected static readonly int ThrowKey = Animator.StringToHash("throw");
        private int _startSpeed;

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            Sounds = GetComponent<PlaySoundsComponent>();
            _startSpeed = _speed;
        }

        public void SetDirection(Vector2 direction)
        {
            Direction = direction;
        }

        public void SetSpeed(int speed) => _speed = speed;
        public void ResetSpeed() { _speed = _startSpeed; }

        protected virtual void Update()
        {
            IsGrounded = _groundCheck.IsTouchingLayer;
        }

        private void FixedUpdate()
        {
            var xVelocity = Direction.x * _speed;
            var yVelocity = CalculateVelocity();
            Rigidbody.velocity = new Vector2(xVelocity, yVelocity);

            Animator.SetBool(IsGroundedKey, IsGrounded);
            Animator.SetFloat(VerticalVelocityKey, Rigidbody.velocity.y);
            Animator.SetBool(IsRunningKey, Direction.x != 0);

            UpdateSpriteDirection(Direction);
        }

        protected virtual float CalculateVelocity()
        {
            var yVelocity = Rigidbody.velocity.y;
            var isJumpPressing = Direction.y > 0;

            if (IsGrounded)
            {
                if (_fallSpeed > _minFallSpeed)
                {
                    Particles.Spawn("Fall");
                }

                _fallSpeed = 0f;
            }
            else
            {
                float currentSpeed = Mathf.Abs(Rigidbody.velocity.y);
                if (currentSpeed > _fallSpeed)
                {
                    _fallSpeed = currentSpeed;
                }
            }

            if (isJumpPressing)
            {
                var isFalling = Rigidbody.velocity.y <= 0.001f;
                if (!isFalling) return yVelocity;

                yVelocity = isFalling ? CalculateJumpVelocity(yVelocity) : yVelocity;
            }
            else if (Rigidbody.velocity.y > 0)
            {
                yVelocity *= 0.5f;
            }

            return yVelocity;
        }


        protected virtual float CalculateJumpVelocity(float yVelocity)
        {
            if (IsGrounded)
            {
                yVelocity = JumpSpeed;
                DoJumpVfx();
            }

            return yVelocity;
        }

        protected void DoJumpVfx()
        {
            Particles.Spawn("Jump");
            Sounds.Play("Jump");
        }

        public void UpdateSpriteDirection(Vector2 direction)
        {
            var multiplier = _invertScale ? -1 : 1;
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(multiplier, 1, 1);
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1 * multiplier, 1, 1);
            }
        }

        public virtual void TakeDamage()
        {
            Animator.SetTrigger(HitKey);
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, _damageJumpSpeed);
        }

        public virtual void Attack()
        {
            Animator.SetTrigger(AttackKey);
            Sounds.Play("Melee");
        }

        public void OnAttackApplying()
        {
            _attackRange.Check();
        }
    }
}