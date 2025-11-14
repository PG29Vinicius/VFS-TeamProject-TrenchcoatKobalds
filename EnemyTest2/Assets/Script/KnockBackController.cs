using UnityEngine;

public class KnockBackController : MonoBehaviour
{
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        float knockbackForce = 0f;

        if (other.name == "KickHitbox")
        {
            knockbackForce = 600f;
        }
        else if (other.name == "PunchHitbox")
        {
            knockbackForce = 300f;
        }
        else
        {
            return;
        }

        // Damage
        EnemyController enemy = GetComponent<EnemyController>();
        if (enemy) enemy.TakeDamage(1);

        // Push back
        Vector3 direction = (transform.position - other.transform.position).normalized;
        _rb.AddForce(direction * knockbackForce);
        _rb.AddForce(Vector3.up * 200f);
    }
}
