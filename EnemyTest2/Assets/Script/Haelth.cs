using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField][Tooltip("The maximum health points")] private int _maxHealth = 100;

    private int _currentHealth;

    public float HeathPercentage => (float)_currentHealth / (float)_maxHealth;

    void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log("Health: " + _currentHealth);

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Died!");
    }
}