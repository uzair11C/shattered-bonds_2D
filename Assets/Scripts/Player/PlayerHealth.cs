using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
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
        Debug.Log("Player died!");
        // TODO: Respawn or show Game Over screen
    }
}
