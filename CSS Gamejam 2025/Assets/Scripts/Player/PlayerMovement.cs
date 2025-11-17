using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private const float GroundedLaunchVelocityThreshold = -0.1f;

    private const float CoyoteTime = 0.1f;
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Landing = Animator.StringToHash("Landing");
    private static readonly int Dash = Animator.StringToHash("Dash");
    private static readonly int InAir = Animator.StringToHash("In Air");
    private static readonly int Death1 = Animator.StringToHash("Death");
    private static readonly int Resurrect = Animator.StringToHash("Resurrect");
    private static readonly int ResurrectionTime = Animator.StringToHash("Resurrection Time");
    private static readonly int ResurrectionFinishTrigger = Animator.StringToHash("Resurrect Finish");
    [SerializeField] private float maxVel = 25.0f;
    [SerializeField] private float horizontalSpeed = 50.0f;
    [SerializeField] private float jumpForce = 800.0f;
    [SerializeField] private float easeIn = 0.3f;
    [SerializeField] private float easeOut = 0.6f;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private float landingThreshold = 2.0f;

    [SerializeField] private float dashSpeed = 100.0f;
    [SerializeField] private float dashRechargeSeconds = 1.0f;

    public Vector2 aim = Vector2.right;

    [SerializeField] private AudioClip landingSound;

    [SerializeField] private float jumpBufferTime = 0.12f;

    [SerializeField] private float grenadeCooldown = 10.0f;
    private Animator _animator;

    private int _direction = 1;

    private bool _inputHeld;

    private bool _isGrounded = true;
    private bool _isInverted;
    private bool _isJumping;
    private bool _isLanding;
    private float _lastDash;

    private float _lastGrenade;
    private float _lastGroundedTime = float.NegativeInfinity;

    private float _lastJumpPressTime = float.NegativeInfinity;

    private Vector2 _lastSafePos;
    private float _originalGravity;
    private float _originalMaxVelocity;
    private Rigidbody2D _rb;
    private AudioSource _sfx;

    private SpriteRenderer _spriteRenderer;

    private Vector2 _targetVel;
    public PlayerLevelManager LevelManager { private set; get; }


    private void Awake()
    {
        _sfx = GameObject.FindWithTag("SFX").GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
        _rb.sharedMaterial = Instantiate(_rb.sharedMaterial);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (_isGrounded) _lastGroundedTime = Time.time;

        // Check if touching ground
        var ray = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, LayerMask.GetMask("Platform"));
        if (ray.collider && !_isJumping)
        {
            _isGrounded = true;
            _animator.SetBool(InAir, false);
            _isLanding = false;
            if (!ray.collider.CompareTag("death") && !ray.collider.CompareTag("unsafe")) _lastSafePos = ray.point;
        }
        else
        {
            _isGrounded = false;
            _animator.SetBool(InAir, true);
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

                // only play if it was a significant fall
                if (!_sfx.isPlaying && Mathf.Abs(_rb.linearVelocity.y) > 15.0f)
                {
                    _sfx.clip = landingSound;
                    _sfx.pitch = Random.Range(0.7f, 1.3f);
                    _sfx.Play();
                }
            }
        }

        TryConsumeBufferedJump();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Respawn")) LevelManager.PlayerDeathImmediate();
        if (other.gameObject.CompareTag("death")) LevelManager.PlayerDeath();
    }

    public float GetXPos()
    {
        return _rb.transform.position.x;
    }

    public void SetLevelManager(PlayerLevelManager levelManager)
    {
        LevelManager = levelManager;
    }

    public void OnMove(InputValue value)
    {
        if (LevelManager.Dead) return;
        var v = value.Get<Vector2>().normalized;
        if (_isInverted) v *= -1;

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
        if (LevelManager.Dead) return;

        if (!value.isPressed)
        {
            _rb.AddForceY(-_rb.linearVelocity.y * easeIn, ForceMode2D.Impulse);
            return;
        }


        _lastJumpPressTime = Time.time;
        TryConsumeBufferedJump();
    }


    private void TryConsumeBufferedJump()
    {
        if (Time.time - _lastJumpPressTime > jumpBufferTime) return; // if jump buffer expired
        if (_isJumping) return; // already jumping
        if (!_isGrounded)
            if (Time.time - _lastGroundedTime > CoyoteTime)
                return; // if coyote time expired

        if (_rb.linearVelocity.y < GroundedLaunchVelocityThreshold) return;

        PerformJump();
    }

    private void PerformJump()
    {
        _animator.SetTrigger(Jump);
        _rb.AddForceY(jumpForce);
        _isGrounded = false;
        _isJumping = true;
        _lastJumpPressTime = float.NegativeInfinity;
    }

    public void Teleport(Vector2 pos)
    {
        _rb.transform.position = pos;
        _rb.linearVelocity = Vector2.zero;
        _isJumping = false;
    }

    public float GetSpeed()
    {
        return maxVel;
    }

    public void SetSpeed(float speed)
    {
        maxVel = speed;
    }

    public void Respawn()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void Death(float deathDuration, float resurrectionDuration)
    {
        _rb.linearVelocity = Vector2.zero;
        _inputHeld = false;
        _isJumping = false;
        _lastJumpPressTime = float.NegativeInfinity;
        _lastGroundedTime = float.NegativeInfinity;
        _rb.bodyType = RigidbodyType2D.Static;
        _animator.SetTrigger(Death1);
        _animator.SetFloat(ResurrectionTime, -1.0f / resurrectionDuration);
        Invoke(nameof(DeathFinish), deathDuration);
    }

    public void DeathImmediate(float resurrectionDuration)
    {
        _rb.linearVelocity = Vector2.zero;
        _inputHeld = false;
        _rb.bodyType = RigidbodyType2D.Static;
        _animator.SetFloat(ResurrectionTime, -1.0f / resurrectionDuration);
        DeathFinish();
        Invoke(nameof(ResurrectionFinish), resurrectionDuration);
    }

    private void ResurrectionFinish()
    {
        _animator.SetTrigger(ResurrectionFinishTrigger);
    }

    private void DeathFinish()
    {
        Teleport(_lastSafePos);
        _animator.SetTrigger(Resurrect);
    }

    public void OnDash(InputValue value)
    {
        if (!(Time.time - _lastDash > dashRechargeSeconds) || LevelManager.Dead) return;
        _lastDash = Time.time;
        _rb.AddForceX(dashSpeed * _direction);
        _animator.SetTrigger(Dash);
    }

    public void InvertControls()
    {
        _isInverted = true;
    }

    public void RevertControls()
    {
        _isInverted = false;
    }

    public void OnAim(InputValue value)
    {
        var temp = value.Get<Vector2>().normalized;
        if (temp.sqrMagnitude != 0) aim = temp;
    }

    public void ApplyForce(Vector2 force)
    {
        _rb.AddForce(force);
    }

    public void OnGrenade()
    {
        if (Time.time - _lastGrenade > grenadeCooldown)
        {
            _lastGrenade = Time.time;
            LevelManager.ShootProjectile(LevelManager.grenadePrefab, aim);
        }
    }
}