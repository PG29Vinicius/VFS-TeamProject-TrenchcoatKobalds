using UnityEngine;

public class RangeProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField][Tooltip("The amount of damage this projectile deals to the player")] private int _damage = 20;

    /// <summary>
    /// Sets the damage value for this projectile.
    /// </summary>
    /// <param name="damage">The amount of damage to deal.</param>
    public void SetDamage(int damage)
    {
        _damage = damage;
        Debug.Log("Projectile damage set to: " + _damage);
    }

    /// <summary>
    /// Handles collision with other objects, dealing damage to the player and destroying the projectile.
    /// </summary>
    /// <param name="collision">The collision information.</param>
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Projectile hit: " + collision.gameObject.name + " with tag: " + collision.gameObject.tag);

        // Use CompareTag instead of checking name for better performance
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit player! Dealing damage...");
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(_damage);
                Debug.Log("Damage applied!");
            }
            else
            {
                Debug.LogError("PlayerController component not found!");
            }
        }

        // Destroy projectile on any collision
        Destroy(gameObject);
    }
}