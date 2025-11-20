using UnityEngine;

public class TankEnemyController : MonoBehaviour
{
    [Header("Tank Enemy Stats")]
    [SerializeField] private int _health = 100;
    [SerializeField] private float _speed = 2.5f;
    [SerializeField] private int _damage = 15;
    [SerializeField] private float _attackDistance = 0.8f;
    [SerializeField] private float _attackCooldown = 1.5f;
    
    private Transform _player;
    private float _attackTimer = 0f;
    private bool _isAttacking = false;
    
    void Start()
    {
        GetComponent<Rigidbody>().mass = 5;
        
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
            
            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }
            
            if (distance <= _attackDistance && _attackTimer <= 0 && !_isAttacking)
            {
                Attack();
            }
            else if (distance > _attackDistance)
            {
                ChasePlayer();
            }
            
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
        
        Debug.Log("Tank Enemy attacks!");
        
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
    void OnCollisionStay(Collision collision)
{
    if (collision.gameObject.name == "Player" && _attackTimer <= 0 && !_isAttacking)
    {
        Attack();
    }
}
}