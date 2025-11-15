using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Landing = Animator.StringToHash("Landing");
    [SerializeField] private float maxVel = 25.0f;
    [SerializeField] private float horizontalSpeed = 100.0f;
    [SerializeField] private float jumpForce = 100.0f;
    [SerializeField] private float easeIn = 0.3f;
    [SerializeField] private float easeOut = 0.6f;
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private float landingThreshold = 2.0f;
    private Animator _animator;
    private float originalMaxVelocity;

    private bool _isGrounded = true;
    private bool _isJumping;
    private bool _isLanding;

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
            _isLanding = false;
            _lastGroundedPos = _rb.transform.position;
        }
        else
        {
            _isGrounded = false;
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

        var lerpSpeed = _targetVel.x == 0.0 ? easeOut : easeIn;
        var vel = Vector2.Lerp(_rb.linearVelocity, _targetVel, lerpSpeed);
        vel.y = _rb.linearVelocity.y;
        _rb.linearVelocity = vel;
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

        if (v.x != 0) _spriteRenderer.flipX = v.x < 0;

        _targetVel = new Vector2(horizontalSpeed * v.x, 0);
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

    public void AddSpeed(int speed)
    {
        originalMaxVelocity = maxVel;
        maxVel *= speed;
        Invoke(nameof(ResetSpeed), 5);
    }

    private void ResetSpeed()
    {
        maxVel = originalMaxVelocity;
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
}