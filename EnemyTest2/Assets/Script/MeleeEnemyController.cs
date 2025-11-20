using UnityEngine;

public class MeleeEnemyController : MonoBehaviour
{
    [Header("Melee Enemy Stats")]
    [SerializeField] private int _health = 5;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _attackDistance = 0.5f;
    [SerializeField] private float _attackCooldown = 1f;
    
    private Transform _player;
    private float _attackTimer = 0f;
    private bool _isAttacking = false;
    
    void Start()
    {
        transform.localScale = new Vector3(0.3f, 1.8f, 0.3f);
        
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
    }
    
    void Update()
    {
        if (_player != null)
        {
            float distance = Vector3.Distance(transform.position, _player.position);
            
            // Update attack timer
            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }
            
            // Check if can attack
            if (distance <= _attackDistance && _attackTimer <= 0 && !_isAttacking)
            {
                Attack();
            }
            else if (distance > _attackDistance)
            {
                ChasePlayer();
            }
            
            // Always look at player
            transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));
        }
    }
    
    void ChasePlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * _speed * Time.deltaTime;
    }
    
    void Attack()
    {
        _isAttacking = true;
        _attackTimer = _attackCooldown;
        
        //Debug.Log("Melee Enemy attacks!");
        
        // Deal damage
        PlayerController playerController = _player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(_damage);
        }
        
        _isAttacking = false;
    }
    
    public void TakeDamage(int damage)
    {
        _health -= damage;
        
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }
}