using UnityEngine;

public class AttackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _kickHitbox;
    [SerializeField] private GameObject _punchHitbox;

    [Header("Attack Settings")]
    [SerializeField] private float _kickDuration = 0.2f;
    [SerializeField] private float _punchDuration = 0.15f;

    [Header("Knockback Forces")]
    [SerializeField] private float _kickForce = 600f;
    [SerializeField] private float _punchForce = 300f;
    [SerializeField] private float _upForce = 200f;
    [SerializeField] private float _speedMultiplier = 1.0f; //How much speed affects knockback

    [Header("Damage")]
    [SerializeField] private int _baseDamage = 1;
    [SerializeField] private int _maxBonusDamage = 4;

    private bool _canAttack = true;
    private string _currentAttack = "";
    private Rigidbody _playerRb;

    void Start()
    {
        if (_kickHitbox == null) _kickHitbox = GameObject.Find("KickHitbox");
        if (_punchHitbox == null) _punchHitbox = GameObject.Find("PunchHitbox");

        if (_kickHitbox != null) _kickHitbox.SetActive(false);
        if (_punchHitbox != null) _punchHitbox.SetActive(false);
    }

    void Update()
    {
        if (!_canAttack) return;

        // F = Kick
        if (Input.GetKeyDown(KeyCode.F))
        {
            Kick();
            Debug.Log("Kick!!");
        }

        // G = Punch
        if (Input.GetKeyDown(KeyCode.G))
        {
            Punch();
            Debug.Log("Punch!!");
        }
    }

    void Kick()
    {
        if (_kickHitbox == null) return;

        _currentAttack = "Kick";
        _canAttack = false;
        _kickHitbox.SetActive(true);
        Invoke(nameof(EndKick), _kickDuration);
    }

    void EndKick()
    {
        if (_kickHitbox != null) _kickHitbox.SetActive(false);
        _currentAttack = "";
        _canAttack = true;
    }

    void Punch()
    {
        if (_punchHitbox == null) return;

        _currentAttack = "Punch";
        _canAttack = false;
        _punchHitbox.SetActive(true);
        Invoke(nameof(EndPunch), _punchDuration);
    }

    void EndPunch()
    {
        if (_punchHitbox != null) _punchHitbox.SetActive(false);
        _currentAttack = "";
        _canAttack = true;
    }

    // Apply knockback to enemy (called by HitDetector on hitbox)
    public void ApplyKnockback(GameObject enemy, Vector3 hitPosition)
    {
        Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
        EnemyController enemyController = enemy.GetComponent<EnemyController>();

        if (enemyRb == null || enemyController == null) return;

        // Damage enemy
        enemyController.TakeDamage(1);

        // Calculate knockback
        float force = _currentAttack == "Kick" ? _kickForce : _punchForce;
        Vector3 direction = (enemy.transform.position - hitPosition).normalized;

        // Apply force
        enemyRb.AddForce(direction * force);
        enemyRb.AddForce(Vector3.up * _upForce);
    }
}