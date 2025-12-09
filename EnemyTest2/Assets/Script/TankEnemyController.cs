using UnityEngine;

public class TankEnemyController : MonoBehaviour
{
    [Header("Tank Enemy Stats")]
    [SerializeField][Tooltip("The movement speed of the tank enemy in units per second")] private float _speed = 2.5f;
    [SerializeField][Tooltip("The amount of damage dealt to the player per attack")] private int _damage = 15;
    [SerializeField][Tooltip("The distance at which the tank enemy can attack the player")] private float _attackDistance = 0.8f;
    [SerializeField][Tooltip("Time in seconds between consecutive attacks")] private float _attackCooldown = 1.5f;

    private float _attackTimer = 0f;
    private Transform _player;
    private bool _isAttacking = false;


    // Initializes the tank enemy by setting its mass and finding the player reference.
    void Start()
    {
        GetComponent<Rigidbody>().mass = 5;

        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
    }

    // Updates the tank enemy each frame, managing attack timer and deciding whether to chase or attack the player.
    void Update()
    {
        if (_player != null)
        {
            float distance = Vector3.Distance(transform.position, _player.position);

            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }

            /*if (distance <= _attackDistance && _attackTimer <= 0 && !_isAttacking)
            {
                Attack();
            }*/
            else if (distance > _attackDistance)
            {
                ChasePlayer();
            }

            transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));
        }
    }

    /// <summary>
    /// Chases the player by moving towards their position.
    /// </summary>
    void ChasePlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * _speed * Time.deltaTime;
    }

    /// <summary>
    /// Attacks the player, dealing damage and resetting the attack timer.
    /// </summary>
    /*void Attack()
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
    }*/

    /// <summary>
    /// Destroys the tank enemy instantly when hit.
    /// </summary>
/*    public void Die()
    {
        Debug.Log("Tank Enemy died!");
        Destroy(gameObject);
    }*/

    /// <summary>
    /// Handles collision stay events to trigger attacks on the player.
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionStay(Collision collision)
    {
      /*  if (collision.gameObject.name == "Player" && _attackTimer <= 0 && !_isAttacking)
        {
            Attack();
        }*/
    }
}