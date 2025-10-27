using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private int _speed;
    [SerializeField] private int _jumpSpeed;
    [SerializeField] private LayerCheck _groundCheck;

    private Rigidbody2D _rigidbody;
    private Vector2 _direction;

    private Animator _animator;
    private static readonly int IsGroundedKey = Animator.StringToHash("is_grounded");
    private static readonly int IsRunningKey = Animator.StringToHash("is_running");
    private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical_velocity");

    private SpriteRenderer _sprite;

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
        _rigidbody.velocity = new Vector2(_direction.x * _speed, _rigidbody.velocity.y);

        var isJumping = _direction.y > 0;
        var isGrounded = IsGrounded();

        if (isJumping)
        {
            if (isGrounded)
            {
                _rigidbody.AddForce(Vector2.up * _jumpSpeed, ForceMode2D.Impulse);
            }
        }
        else if (_rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }

        _animator.SetBool(IsGroundedKey, isGrounded);
        _animator.SetFloat(VerticalVelocityKey, _rigidbody.velocity.y);
        _animator.SetBool(IsRunningKey, _direction.x != 0);

        UpdateSpriteDirection();
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
}
