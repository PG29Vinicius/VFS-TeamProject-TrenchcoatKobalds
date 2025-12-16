using UnityEngine;

public class MeleeEnemyController : MonoBehaviour
{
    [Header("Melee Enemy Stats")]
    [SerializeField][Tooltip("The movement speed of the melee enemy in units per second")] private float _speed = 5f;
    [SerializeField][Tooltip("The amount of damage dealt to the player per attack")] private int _damage = 20;
    [SerializeField][Tooltip("Time in seconds between consecutive attacks")] private float _attackCooldown = 1f;

    [Header("Detection Ranges")]
    [SerializeField][Tooltip("The radius within which the enemy can detect the player")] private float _searchRange = 10f;
    [SerializeField][Tooltip("The radius within which the enemy can attack the player")] private float _attackRange = 2f;

    private Transform _player;
    private float _attackTimer = 0f;
    private bool _isAttacking = false;
    private bool _playerInSearchRange = false;
    private bool _playerInAttackRange = false;

    // Initializes the melee enemy by finding the player and setting up detection zones.
    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }

        SetupDetectionZones();
    }

    /// <summary>
    /// Creates trigger colliders for search and attack ranges.
    /// </summary>
    void SetupDetectionZones()
    {
        // Create search range collider
        GameObject searchZone = new GameObject("SearchZone");
        searchZone.transform.SetParent(transform);
        searchZone.transform.localPosition = Vector3.zero;
        searchZone.layer = gameObject.layer;

        SphereCollider searchCollider = searchZone.AddComponent<SphereCollider>();
        searchCollider.isTrigger = true;
        searchCollider.radius = _searchRange;

        SearchRangeDetector searchDetector = searchZone.AddComponent<SearchRangeDetector>();
        searchDetector.Initialize(this);

        // Create attack range collider
        GameObject attackZone = new GameObject("AttackZone");
        attackZone.transform.SetParent(transform);
        attackZone.transform.localPosition = Vector3.zero;
        attackZone.layer = gameObject.layer;

        SphereCollider attackCollider = attackZone.AddComponent<SphereCollider>();
        attackCollider.isTrigger = true;
        attackCollider.radius = _attackRange;

        AttackRangeDetector attackDetector = attackZone.AddComponent<AttackRangeDetector>();
        attackDetector.Initialize(this);
    }

    // Updates the melee enemy each frame, managing attack timer and deciding whether to chase or attack the player.
    void Update()
    {
        if (_player != null)
        {
            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }

            // Only chase if player is in search range but not in attack range
            if (_playerInSearchRange && !_playerInAttackRange)
            {
                ChasePlayer();
                transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));
            }
            // Attack if player is in attack range
            else if (_playerInAttackRange && _attackTimer <= 0 && !_isAttacking)
            {
                transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));
                Attack();
            }
        }
    }

    /// <summary>
    /// Moves the melee enemy towards the player's position.
    /// </summary>
    void ChasePlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * _speed * Time.deltaTime;
    }

    /// <summary>
    /// Executes a melee attack on the player, dealing damage and resetting the attack timer.
    /// </summary>
    void Attack()
    {
        _isAttacking = true;
        _attackTimer = _attackCooldown;

        Debug.Log("Melee Enemy attacks!");

        PlayerController playerController = _player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(_damage);
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
    /// Destroys the melee enemy when taking damage.
    /// </summary>
    public void TakeDamage(int damage)
    {
        Debug.Log("Melee Enemy died!");
        Destroy(gameObject);
    }

    /// <summary>
    /// Visualizes the search and attack ranges in the editor.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draw search range in yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _searchRange);

        // Draw attack range in red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}