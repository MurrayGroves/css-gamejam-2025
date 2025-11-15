using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Landing = Animator.StringToHash("Landing");
    private static readonly int Dash = Animator.StringToHash("Dash");
    private static readonly int Property = Animator.StringToHash("In Air");
    [SerializeField] private float maxVel = 25.0f;
    [SerializeField] private float horizontalSpeed = 100.0f;
    [SerializeField] private float jumpForce = 100.0f;
    [SerializeField] private float easeIn = 0.3f;
    [SerializeField] private float easeOut = 0.6f;
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private float landingThreshold = 2.0f;

    [SerializeField] private float dashSpeed = 100.0f;
    [SerializeField] private float dashRechargeSeconds = 1.0f;
    private Animator _animator;

    private int _direction = 1;

    private bool _inputHeld;

    private bool _isGrounded = true;
    private bool _isJumping;
    private bool _isLanding;

    private float _lastDash;

    private Vector2 _lastGroundedPos;
    private PlayerLevelManager _levelManager;
    private Rigidbody2D _rb;

    private SpriteRenderer _spriteRenderer;

    private Vector2 _targetVel;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // Check if touching ground
        var ray = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, LayerMask.GetMask("Platform"));
        if (ray.collider && !_isJumping)
        {
            _isGrounded = true;
            _animator.SetBool(Property, false);
            _isLanding = false;
            _lastGroundedPos = _rb.transform.position;
        }
        else
        {
            _isGrounded = false;
            _animator.SetBool(Property, true);
        }

        if (_inputHeld)
        {
            if ((_direction == -1 && _rb.linearVelocity.x > -maxVel) ||
                (_direction == 1 && _rb.linearVelocity.x < maxVel))
                _rb.AddForceX(horizontalSpeed * _direction);
        }
        else
        {
            _rb.AddForceX(-_rb.linearVelocity.x * easeOut);
        }

        // Check for landing animation
        if (!_isGrounded && _rb.linearVelocity.y < 0 && !_isLanding)
        {
            var landingRay = Physics2D.Raycast(transform.position, Vector2.down, landingThreshold,
                LayerMask.GetMask("Platform"));
            if (landingRay.collider)
            {
                _animator.SetTrigger(Landing);
                _isLanding = true;
                _isJumping = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger enter");
        if (other.gameObject.CompareTag("Respawn"))
        {
            Debug.Log("Collided with death collider");
            _levelManager.PlayerDeath();
        }
    }

    public float GetXPos()
    {
        return _rb.transform.position.x;
    }

    public void SetLevelManager(PlayerLevelManager levelManager)
    {
        _levelManager = levelManager;
    }

    public void OnMove(InputValue value)
    {
        var v = value.Get<Vector2>().normalized;

        _animator.SetBool(Move, v.x != 0);

        if (v.x != 0)
        {
            _inputHeld = true;
            if (v.x < 0)
            {
                _spriteRenderer.flipX = true;
                _direction = -1;
            }
            else
            {
                _spriteRenderer.flipX = false;
                _direction = 1;
            }
        }
        else
        {
            _inputHeld = false;
        }
    }

    public void OnJump(InputValue value)
    {
        // Jump
        if (!_isGrounded || _isJumping) return;
        _animator.SetTrigger(Jump);
        _rb.AddForceY(jumpForce);
        _isGrounded = false;
        _isJumping = true;
    }

    public void Teleport(Vector2 pos)
    {
        _rb.transform.position = pos;
        _rb.linearVelocity = Vector2.zero;
    }

    public void Respawn()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void Death()
    {
        _rb.bodyType = RigidbodyType2D.Static;
        Teleport(_lastGroundedPos);
    }

    public void OnDash(InputValue value)
    {
        if (!(Time.time - _lastDash > dashRechargeSeconds)) return;
        _lastDash = Time.time;
        _rb.AddForceX(dashSpeed * _direction);
        _animator.SetTrigger(Dash);
    }
}