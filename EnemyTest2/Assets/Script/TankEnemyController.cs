using UnityEngine;

public class TankEnemyController : MonoBehaviour
{
    [Header("Tank Enemy Stats")]
    [SerializeField][Tooltip("The movement speed of the tank enemy in units per second")] private float _speed = 2.5f;
    [SerializeField][Tooltip("The distance at which the tank enemy can attack the player")] private float _attackDistance = 0.8f;

    private Transform _player;

    // Initializes the tank enemy by setting its mass and finding the player reference.
    void Start()
    {
        GetComponent<Rigidbody>().mass = 5;
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
    }

    // Updates the tank enemy each frame, chasing the player.
    void Update()
    {
        if (_player != null)
        {
            float distance = Vector3.Distance(transform.position, _player.position);

            if (distance > _attackDistance)
            {
                ChasePlayer();
            }

            transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));
        }
    }

    /// <summary>
    /// Chases the player by moving towards their position.
    /// </summary>
    void ChasePlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * _speed * Time.deltaTime;
    }

    /// <summary>
    /// Destroys the tank enemy instantly when hit.
    /// </summary>
    public void Die()
    {
        Debug.Log("Tank Enemy died!");
        Destroy(gameObject);
    }
}