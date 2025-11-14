using UnityEngine;

public class HitDetector : MonoBehaviour
{
    private AttackController _attackController;

    void Start()
    {
        _attackController = GetComponentInParent<AttackController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _attackController.ApplyKnockback(other.gameObject, transform.position);
        }
    }
}