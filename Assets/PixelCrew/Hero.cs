using UnityEngine;


namespace PixelCrew
{
    public class Hero : MonoBehaviour
    {
        [SerializeField] private int _speed;
        [SerializeField] private int _jumpSpeed;
        [SerializeField] private int _damageJumpSpeed;
        [SerializeField] private LayerCheck _groundCheck;
        [SerializeField] private float _interactionRadius;
        [SerializeField] private LayerMask _interactionLayer;

        private Rigidbody2D _rigidbody;
        private Vector2 _direction;
        private readonly Collider2D[] _interactionResults = new Collider2D[1];

        private Animator _animator;
        private static readonly int IsGroundedKey = Animator.StringToHash("is_grounded");
        private static readonly int IsRunningKey = Animator.StringToHash("is_running");

        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical_velocity");
        private static readonly int HitKey = Animator.StringToHash("hit");

        private SpriteRenderer _sprite;
        private bool _isGrounded;
        private bool _allowDoubleJump;

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _sprite = GetComponent<SpriteRenderer>();
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
        }

        private void Update()
        {
            _isGrounded = IsGrounded();
        }

        private float CalculateVelocity()
        {
            var yVelocity = _rigidbody.velocity.y;
            var isJumpPressing = _direction.y > 0;

            if (_isGrounded) _allowDoubleJump = true;

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
            }
            else if (_allowDoubleJump)
            {
                yVelocity = _jumpSpeed;
                _allowDoubleJump = false;
            }
            return yVelocity;
        }

        private void UpdateSpriteDirection()
        {
            if (_direction.x > 0)
            {
                _sprite.flipX = false;
            }
            else if (_direction.x < 0)
            {
                _sprite.flipX = true;
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
    }
}
