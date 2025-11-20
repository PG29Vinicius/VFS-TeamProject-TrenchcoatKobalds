using UnityEngine;

public class AttackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _kickHitbox;

    [Header("Attack Settings")]
    [SerializeField] private float _kickDuration = 0.2f;
    [SerializeField] private int _kickDamage = 1;

    [Header("Knockback Forces")]
    [SerializeField] private float _kickForce = 600f;
    [SerializeField] private float _upForce = 200f;
    [SerializeField] private float _speedMultiplier = 1.0f;

    private bool _canAttack = true;
    private Rigidbody _playerRb;

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

    void EndKick()
    {
        if (_kickHitbox != null)
        {
            _kickHitbox.SetActive(false);
        }

        _canAttack = true;
    }

    // Apply knockback and damage
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