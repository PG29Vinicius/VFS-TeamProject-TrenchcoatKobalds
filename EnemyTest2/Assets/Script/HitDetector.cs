using UnityEngine;

public class HitDetector : MonoBehaviour
{
    private AttackController _attackController;

    // Initializes the HitDetector by setting up a reference to the AttackController.
    void Start()
    {
        _attackController = GetComponentInParent<AttackController>();
    }

    /// <summary>
    /// Detects collisions with enemy objects and applies knockback using the AttackController.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _attackController.ApplyKnockback(other.gameObject, transform.position);
        }
    }
}