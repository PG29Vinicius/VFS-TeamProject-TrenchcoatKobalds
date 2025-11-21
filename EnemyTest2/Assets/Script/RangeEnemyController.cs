using UnityEngine;

public class RangeEnemyController : MonoBehaviour
{
    [Header("Range Enemy Stats")]
    [SerializeField][Tooltip("The maximum health points of the range enemy")] private int _health = 3;
    [SerializeField][Tooltip("The amount of damage dealt to the player per attack")] private int _damage = 20;
    [SerializeField][Tooltip("The maximum distance from which the range enemy can attack the player")] private float _attackDistance = 20f;
    [SerializeField][Tooltip("Time in seconds between consecutive attacks")] private float _attackCooldown = 2f;
    
    private Transform _player;
    private float _attackTimer = 0f;
    private bool _isAttacking = false;

    //Initializes the range enemy by finding and storing a reference to the player.
    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
    }

    // Updates the range enemy each frame, checking distance to player and attacking when in range.
    void Update()
    {
        if (_player != null)
        {
            float distance = Vector3.Distance(transform.position, _player.position);
            
            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }
            
            if (distance <= _attackDistance)
            {
                transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));
                
                if (_attackTimer <= 0 && !_isAttacking)
                {
                    Attack();
                }
            }
        }
    }
    
    /// <summary>
    /// Attacks the player, dealing damage and resetting the attack timer.
    /// </summary>
    void Attack()
    {
        _isAttacking = true;
        _attackTimer = _attackCooldown;
        
        Debug.Log("Range Enemy shoots!");
        
        PlayerController playerController = _player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(_damage);
        }
        
        _isAttacking = false;
    }
    
    /// <summary>
    /// Reduces the range enemy's health by the specified damage amount and destroys it if health reaches zero.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    public void TakeDamage(int damage)
    {
        _health -= damage;
        
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handles collision stay events to trigger attacks on the player.
    /// </summary>
    /// <param name="collision">The collision information.</param>
    void OnCollisionStay(Collision collision)
{
    if (collision.gameObject.name == "Player" && _attackTimer <= 0 && !_isAttacking)
    {
        Attack();
    }
}
}