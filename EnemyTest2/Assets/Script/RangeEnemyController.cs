using UnityEngine;

public class RangeEnemyController : MonoBehaviour
{
    [Header("Range Enemy Stats")]
    [SerializeField] private int _health = 3;
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _attackDistance = 20f;
    [SerializeField] private float _attackCooldown = 2f;
    
    private Transform _player;
    private float _attackTimer = 0f;
    private bool _isAttacking = false;
    
    void Start()
    {
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