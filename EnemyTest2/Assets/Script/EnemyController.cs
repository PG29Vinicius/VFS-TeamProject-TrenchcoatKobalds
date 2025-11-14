using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public int health = 3;
    public float speed = 3f;

    private Transform player;

    void Start()
    {
        // Try to find player
        GameObject playerObj = GameObject.Find("Player");

    }

    void Update()
    {
        ChasePlayer();
    }

    // Follow the player
    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        transform.position += direction * speed * Time.deltaTime;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    // Take damage
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}