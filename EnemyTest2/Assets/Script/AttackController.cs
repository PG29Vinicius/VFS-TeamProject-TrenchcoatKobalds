using UnityEngine;

public class AttackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField][Tooltip("The hitbox GameObject that detects kick collision with enemies")] private GameObject _kickHitbox;

    [Header("Attack Settings")]
    [SerializeField][Tooltip("How long the kick hitbox stays active in seconds")] private float _kickDuration = 0.2f;
    [SerializeField][Tooltip("The amount of damage dealt by a kick attack")] private int _kickDamage = 1;

    [Header("Knockback Forces")]
    [SerializeField][Tooltip("The horizontal force applied to enemies when kicked")] private float _kickForce = 600f;
    [SerializeField][Tooltip("The upward force applied to enemies when kicked")] private float _upForce = 200f;
    [SerializeField][Tooltip("Multiplier for knockback based on player movement speed")] private float _speedMultiplier = 1.0f;

    private bool _canAttack = true;
    private Rigidbody _playerRb;

 
    /// Initializes the AttackController by setting up references to the player's Rigidbody and the kick hitbox.
    void Start()
    {
        _playerRb = GetComponentInParent<Rigidbody>();

        if (_kickHitbox == null)
        {
            _kickHitbox = GameObject.Find("KickHitbox");
        }

        if (_kickHitbox != null)
        {
            _kickHitbox.SetActive(false);
        }
    }


    /// Handles player input for attacks.
    void Update()
    {
        if (!_canAttack)
        {
            return;
    }

        // F = Kick
        if (Input.GetKeyDown(KeyCode.F))
        {
            Kick();
        }
    }

    /// <summary>
    /// Initiates the kick attack by activating the hitbox and setting a timer to end the attack.
    /// </summary>
    void Kick()
    {
        if (_kickHitbox == null)
        {
            return;
        }

        _canAttack = false;
        _kickHitbox.SetActive(true);
        Invoke(nameof(EndKick), _kickDuration);
    }

    /// <summary>
    /// Ends the kick attack by deactivating the hitbox and allowing further attacks.
    /// </summary>
    void EndKick()
    {
        if (_kickHitbox != null)
        {
            _kickHitbox.SetActive(false);
        }

        _canAttack = true;
    }

    /// <summary>
    /// Applies knockback to the specified enemy and deals damage.
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="hitPosition"></param>
    public void ApplyKnockback(GameObject enemy, Vector3 hitPosition)
    {
        Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();

        if (enemyRb == null)
        {
            return;
        }

        // Apply damage to enemy
        MeleeEnemyController meleeEnemy = enemy.GetComponent<MeleeEnemyController>();
        TankEnemyController tankEnemy = enemy.GetComponent<TankEnemyController>();
        RangeEnemyController rangeEnemy = enemy.GetComponent<RangeEnemyController>();

        if (meleeEnemy != null)
        { 
            meleeEnemy.TakeDamage(_kickDamage);
        }
        else if (tankEnemy != null)
        {
            tankEnemy.TakeDamage(_kickDamage);
        }
        else if (rangeEnemy != null)
        {
            rangeEnemy.TakeDamage(_kickDamage);
        }

        // Calculate player speed
        float playerSpeed = _playerRb.linearVelocity.magnitude;
        float speedBonus = 1f + (playerSpeed * _speedMultiplier);

        // Calculate knockback with speed bonus
        float finalForce = _kickForce * speedBonus;
        Vector3 direction = (enemy.transform.position - hitPosition).normalized;

        // Apply force
        enemyRb.AddForce(direction * finalForce);
        enemyRb.AddForce(Vector3.up * _upForce * speedBonus);
    }
}