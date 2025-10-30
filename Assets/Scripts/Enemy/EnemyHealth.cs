using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private HealthBar healthBar;

    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);

        Debug.Log($"{gameObject.name} took {amount} damage! Remaining: {currentHealth}");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            healthBar.SetHealth(currentHealth);
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        // Destroy(gameObject);
    }
}
