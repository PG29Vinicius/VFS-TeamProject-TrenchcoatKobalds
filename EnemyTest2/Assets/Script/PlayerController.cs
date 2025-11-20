using System;
using Unity.Mathematics;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField][Tooltip("The speed of player and based on spreadsheet")] private int _speed = 10; 
    [SerializeField][Tooltip("How fast player control the camera")] private float _mouseSensitivity = 400f;
    
    [Header("Slide")]
    [SerializeField][Tooltip("The parameter of player slide speed")] private float _slideSpeed = 15f;
    [SerializeField][Tooltip("The distance of player can slide")] private float _slideDuration = 0.5f;
    
    [Header("Health")]
    [SerializeField][Tooltip("")] private int _health = 100;  
    
    [Header("Jump")]
    [SerializeField][Tooltip("")] private float _jumpForce = 8f;
    [SerializeField][Tooltip("")] private int _maxJumps = 2;
    
    private Rigidbody _rb;
    private Camera _cam;
    private Vector3 _moveVector;
    private float _xRotation = 0f;
    private bool _isInitialized = false;
    private int _jumpsRemaining;
    private bool _wasGrounded = false;
    private bool _isDead = false;
    
    private bool _isSliding = false;
    private float _slideTimer = 0f;
    private Vector3 _slideDirection;
    
    /// <summary>
    /// Initializes the player controller by setting up references, locking the cursor, and preparing movement variables.
    /// </summary>
    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _cam = GetComponentInChildren<Camera>();
        _moveVector = Vector3.zero;
        
        _xRotation = 0f;
        _cam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Cursor.lockState = CursorLockMode.Locked;
        
        Invoke(nameof(FinishInitialization), 0.1f);
  
    }
    
    /// <summary>
    /// Marks the player controller as initialized, allowing for input processing.
    /// </summary>
    private void FinishInitialization()
    {
        _isInitialized = true;
    }
    
    /// <summary>
    /// Handles player input for movement, camera control, jumping, and sliding.
    /// </summary>
    private void Update()
    {
        // Camera control
        if (_isInitialized)
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;
            
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            
            _cam.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            gameObject.transform.Rotate(Vector3.up * mouseX);
        }
        
        float zMovement = Input.GetAxis("Vertical");
        float xMovement = Input.GetAxis("Horizontal");
        _moveVector = transform.right * xMovement + transform.forward * zMovement;
        
        // Slide input - Left Shift
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isSliding)
        {
            StartSlide();
        }
        
        // Update slide timer
        if (_isSliding)
        {
            _slideTimer -= Time.deltaTime;
            if (_slideTimer <= 0)
            {
                _isSliding = false;
            }
        }
        
        // Jump input - Space
        if (Input.GetKeyDown(KeyCode.Space) && _jumpsRemaining > 0)
        {
            Jump();
        }
        
        CheckGround();
    }
    
    /// <summary>
    /// Handles physics-based movement and sliding mechanics.
    /// </summary>
    private void FixedUpdate()
    {
        if (_isSliding)
        {
            // Keep Y velocity for gravity, only change X and Z
            _rb.linearVelocity = new Vector3(
                _slideDirection.x * _slideSpeed, 
                _rb.linearVelocity.y,  // Keep gravity!
                _slideDirection.z * _slideSpeed
            );
        }
        else
        {
            _rb.AddForce(_moveVector * _speed);
        }
    }
    
    /// <summary>
    /// Handles the jump action, applying upward force and decrementing remaining jumps.
    /// </summary>
    private void Jump()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _jumpsRemaining--;
    }
    
    /// <summary>
    /// Checks if the player is grounded and resets jump count if so.
    /// </summary>
    private void CheckGround()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        
        if (isGrounded && !_wasGrounded)
        {
            _jumpsRemaining = _maxJumps;
        }
        
        _wasGrounded = isGrounded;
    }

    /// <summary>
    /// Initiates the sliding action by setting the slide state, timer, and direction.
    /// </summary>    
    private void StartSlide()
    {
        _isSliding = true;
        _slideTimer = _slideDuration;
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        _slideDirection = (transform.right * x + transform.forward * z).normalized;
        
        if (_slideDirection == Vector3.zero)
        {
            _slideDirection = transform.forward;
        }
    }
    
    /// <summary>
    /// Applies damage to the player and checks for death condition.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (_isDead) return;
        
        _health -= damage;
        Debug.Log("Health: " + _health);
        
        if (_health <= 0)
        {
            Die();
        }
    }
    
   
    /// <summary>
    /// Handles player death by marking the player as dead and stopping movement.
    /// </summary>
    void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector3.zero;
        Debug.Log("You died!");
    }
}