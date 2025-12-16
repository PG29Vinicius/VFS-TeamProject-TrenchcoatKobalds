using UnityEngine;

public class RangeEnemyController : MonoBehaviour
{
    [Header("Range Enemy Stats")]
    [SerializeField][Tooltip("The amount of damage dealt to the player per attack")] private int _damage = 20;
    [SerializeField][Tooltip("Time in seconds between consecutive attacks")] private float _attackCooldown = 2f;
    [SerializeField][Tooltip("Speed of the projectile")] private float _projectileSpeed = 30f;

    [Header("Detection Ranges")]
    [SerializeField][Tooltip("The radius within which the enemy can detect the player")] private float _searchRange = 25f;
    [SerializeField][Tooltip("The length of the rectangular attack zone (forward)")] private float _attackLength = 50f;
    [SerializeField][Tooltip("The width of the rectangular attack zone")] private float _attackWidth = 3f;
    [SerializeField][Tooltip("The height of the rectangular attack zone")] private float _attackHeight = 3f;

    [Header("Projectile")]
    [SerializeField][Tooltip("The projectile prefab to spawn")] private GameObject _projectilePrefab;
    [SerializeField][Tooltip("The spawn point for projectiles")] private Transform _firePoint;

    private Transform _player;
    private float _attackTimer = 0f;
    private bool _isAttacking = false;
    private bool _playerInSearchRange = false;
    private bool _playerInAttackRange = false;

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

        SetupDetectionZones();
    }

    /// <summary>
    /// Creates trigger colliders for search and attack ranges.
    /// </summary>
    void SetupDetectionZones()
    {
        // Create circular search range collider
        GameObject searchZone = new GameObject("SearchZone");
        searchZone.transform.SetParent(transform);
        searchZone.transform.localPosition = Vector3.zero;
        searchZone.layer = gameObject.layer;

        SphereCollider searchCollider = searchZone.AddComponent<SphereCollider>();
        searchCollider.isTrigger = true;
        searchCollider.radius = _searchRange;

        SearchRangeDetector searchDetector = searchZone.AddComponent<SearchRangeDetector>();
        searchDetector.InitializeForRangeEnemy(this);

        // Create rectangular "laser sight" attack range collider
        GameObject attackZone = new GameObject("AttackZone");
        attackZone.transform.SetParent(transform);
        // Position it forward so it extends in front of the enemy
        attackZone.transform.localPosition = new Vector3(0, 0, _attackLength / 2);
        attackZone.layer = gameObject.layer;

        BoxCollider attackCollider = attackZone.AddComponent<BoxCollider>();
        attackCollider.isTrigger = true;
        // Size: width (x), height (y), length (z)
        attackCollider.size = new Vector3(_attackWidth, _attackHeight, _attackLength);

        AttackRangeDetector attackDetector = attackZone.AddComponent<AttackRangeDetector>();
        attackDetector.InitializeForRangeEnemy(this);
    }

    // Updates the range enemy each frame - stationary, only rotates to face player and attacks when in range.
    void Update()
    {
        if (_player != null)
        {
            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }

            // Only look at and attack if player is in search range
            if (_playerInSearchRange)
            {
                transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));

                // Attack if player is in the attack zone (rectangular laser sight)
                if (_playerInAttackRange && _attackTimer <= 0 && !_isAttacking)
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

            if (Physics.Raycast(_firePoint.position, direction, out hit, _attackLength))
            {
                if (hit.collider.CompareTag("Player"))
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
    /// Called when player enters the search range.
    /// </summary>
    public void OnPlayerEnterSearchRange()
    {
        _playerInSearchRange = true;
    }

    /// <summary>
    /// Called when player exits the search range.
    /// </summary>
    public void OnPlayerExitSearchRange()
    {
        _playerInSearchRange = false;
    }

    /// <summary>
    /// Called when player enters the attack range.
    /// </summary>
    public void OnPlayerEnterAttackRange()
    {
        _playerInAttackRange = true;
    }

    /// <summary>
    /// Called when player exits the attack range.
    /// </summary>
    public void OnPlayerExitAttackRange()
    {
        _playerInAttackRange = false;
    }

    /// <summary>
    /// Destroys the range enemy instantly when hit.
    /// </summary>
    public void TakeDamage(int damage)
    {
        Debug.Log("Range Enemy died!");
        Destroy(gameObject);
    }

    /// <summary>
    /// Visualizes the search and attack ranges in the editor.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draw circular search range in yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _searchRange);

        // Draw rectangular attack range in red (laser sight)
        Gizmos.color = Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(new Vector3(0, 0, _attackLength / 2), new Vector3(_attackWidth, _attackHeight, _attackLength));
        Gizmos.matrix = Matrix4x4.identity;
    }
}