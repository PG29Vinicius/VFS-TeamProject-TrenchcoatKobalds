using System;
using Unity.Mathematics;
using UnityEngine;
using TMPro; 

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private int _speed = 10;  // Based on spreadsheet
    [SerializeField] private float _mouseSensitivity = 400f;
    
    [Header("Slide")]
    [SerializeField] private float _slideSpeed = 15f;
    [SerializeField] private float _slideDuration = 0.5f;
    
    [Header("Health")]
    [SerializeField] private int _health = 100;  // From spreadsheet: Player health = 100
    [SerializeField] private TextMeshProUGUI _healthText;
    
    [Header("Jump")]
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private int _maxJumps = 2;
    
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
    
    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _cam = GetComponentInChildren<Camera>();
        _moveVector = Vector3.zero;
        
        _xRotation = 0f;
        _cam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Cursor.lockState = CursorLockMode.Locked;
        
        Invoke(nameof(FinishInitialization), 0.1f);
        UpdateHealthUI();
    }
    
    private void FinishInitialization()
    {
        _isInitialized = true;
    }
    
    private void Update()
    {
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
        
        if (Input.GetKeyDown(KeyCode.Space) && _jumpsRemaining > 0)
        {
            Jump();
        }
        
        CheckGround();
    }
    
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
    
    private void Jump()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _jumpsRemaining--;
    }
    
    private void CheckGround()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        
        if (isGrounded && !_wasGrounded)
        {
            _jumpsRemaining = _maxJumps;
        }
        
        _wasGrounded = isGrounded;
    }
    
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
    
    public void TakeDamage(int damage)
    {
        if (_isDead) return;
        
        _health -= damage;
        UpdateHealthUI();
        Debug.Log("Health: " + _health);
        
        if (_health <= 0)
        {
            Die();
        }
    }
    
    void UpdateHealthUI()
    {
        if (_healthText != null)
        {
            _healthText.text = "Hp: " + _health;
        }
    }
    
    void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector3.zero;
        Debug.Log("You died!");
    }
}