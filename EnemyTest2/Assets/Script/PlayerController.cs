using System;
using Unity.Mathematics;
using UnityEngine;
using TMPro; 

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private int _speed = 10;
    [SerializeField] private float _mouseSensitivity = 400f;

    [SerializeField] private Camera _playerCamera; // reference

    [Header("Health")]
    [SerializeField] private int _health = 100;
    [SerializeField] private TextMeshProUGUI _healthText;

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 8f;    // Jump strength
    [SerializeField] private int _maxJumps = 2;          // Total jumps allowed


    private Rigidbody _rb;
    private Vector3 _moveVector;
    private float _xRotation = 0f;
    private bool _isInitialized = false; // flag
    private int _jumpsRemaining;   // Current jumps left
    private bool _wasGrounded = false;  // Track if was on ground last frame
    private bool _isDead = false;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _moveVector = Vector3.zero;

        // Get camera reference if not assigned
        if (_playerCamera == null)
        {
            _playerCamera = Camera.main;
        }

        // Force reset rotation
        _xRotation = 0f;
        _playerCamera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        Cursor.lockState = CursorLockMode.Locked;

        // Mark as initialized after one frame
        Invoke(nameof(FinishInitialization), 0.1f);

        UpdateHealthUI();
    }

    private void FinishInitialization()
    {
        _isInitialized = true;
    }

    private void Update()
    {
        // Skip mouse input for the first few frames
        if (_isInitialized)
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            _playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            gameObject.transform.Rotate(Vector3.up * mouseX);
        }

        float zMovement = Input.GetAxis("Vertical");
        float xMovement = Input.GetAxis("Horizontal");

        _moveVector = transform.right * xMovement + transform.forward * zMovement;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && _jumpsRemaining > 0)
        {
            Jump();
        }

        // Reset jumps on ground
        CheckGround();

    }

    private void FixedUpdate()
    {
        _rb.AddForce(_moveVector * _speed);
    }

    // Perform jump
    private void Jump()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);  // Reset Y velocity
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);                          // Apply jump force
        _jumpsRemaining--;                                                                 // Use one jump
    }

    // Check if touching ground
    private void CheckGround()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Only reset jumps when LANDING (wasn't grounded before, now is)
        if (isGrounded && !_wasGrounded)
        {
            _jumpsRemaining = _maxJumps;
        }

        _wasGrounded = isGrounded;
    }


    // Take damage from enemies
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

    // Update health text on screen
    void UpdateHealthUI()
    {
        if (_healthText != null)
        {
            _healthText.text = "Hp: " + _health;
        }
    }

    // Player dies
    void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector3.zero;
        Debug.Log("You died!");
    }

}