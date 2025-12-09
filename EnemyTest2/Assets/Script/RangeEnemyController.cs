using UnityEngine;

public class RangeEnemyController : MonoBehaviour
{
    [Header("Range Enemy Stats")]
    [SerializeField][Tooltip("The amount of damage dealt to the player per attack")] private int _damage = 20;
    [SerializeField][Tooltip("The maximum distance from which the range enemy can attack the player")] private float _attackDistance = 50f;
    [SerializeField][Tooltip("Time in seconds between consecutive attacks")] private float _attackCooldown = 2f;
    [SerializeField][Tooltip("Speed of the projectile")] private float _projectileSpeed = 30f;

    [Header("Projectile")]
    [SerializeField][Tooltip("The projectile prefab to spawn")] private GameObject _projectilePrefab;
    [SerializeField][Tooltip("The spawn point for projectiles")] private Transform _firePoint;

    private Transform _player;
    private float _attackTimer = 0f;
    private bool _isAttacking = false;

    // Initializes the range enemy by finding and storing a reference to the player.
    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }

        // Setup fire point if not assigned
        if (_firePoint == null)
        {
            _firePoint = transform;
        }
    }

    // Updates the range enemy each frame - stationary, only rotates to face player and attacks when in range.
    void Update()
    {
        if (_player != null)
        {
            float distance = Vector3.Distance(transform.position, _player.position);

            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }

            // Always look at player when in range
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
    /// Fires a projectile at the player from long range.
    /// </summary>
    void Attack()
    {
        _isAttacking = true;
        _attackTimer = _attackCooldown;

        Debug.Log("Range Enemy shoots!");

        // If projectile prefab exists, spawn it
        if (_projectilePrefab != null)
        {
            GameObject projectile = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);

            // Calculate direction to player
            Vector3 direction = (_player.position - _firePoint.position).normalized;

            // Set projectile velocity
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = direction * _projectileSpeed;
            }

            // Setup projectile damage
            RangeProjectile rangeProjectile = projectile.GetComponent<RangeProjectile>();
            if (rangeProjectile != null)
            {
                rangeProjectile.SetDamage(_damage);
            }

            // Destroy projectile after 5 seconds if it doesn't hit anything
            Destroy(projectile, 5f);
        }
        else
        {
            // Fallback: direct damage via raycast if no projectile prefab
            RaycastHit hit;
            Vector3 direction = (_player.position - _firePoint.position).normalized;

            if (Physics.Raycast(_firePoint.position, direction, out hit, _attackDistance))
            {
                if (hit.collider.gameObject.name == "Player")
                {
                    PlayerController playerController = hit.collider.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.TakeDamage(_damage);
                    }
                }
            }
        }

        _isAttacking = false;
    }

    /// <summary>
    /// Destroys the range enemy instantly when hit.
    /// </summary>
/*    public void Die()
    {
        Debug.Log("Range Enemy died!");
        Destroy(gameObject);
    }
*/
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