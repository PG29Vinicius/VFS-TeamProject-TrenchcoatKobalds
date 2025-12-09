using UnityEngine;

public class RangeProjectile : MonoBehaviour
{
    private int _damage = 20;

    /// <summary>
    /// Sets the damage value for this projectile.
    /// </summary>
    /// <param name="damage">The amount of damage to deal.</param>
    public void SetDamage(int damage)
    {

        _damage = damage;
    }

    /// <summary>
    /// Handles collision with other objects, dealing damage to the player.
    /// </summary>
    /// <param name="collision">The collision information.</param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(_damage);
            }
        }

        // Destroy projectile on any collision
        Destroy(gameObject);
    }
}