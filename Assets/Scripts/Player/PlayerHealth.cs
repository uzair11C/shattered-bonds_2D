using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        // if (healthBar)
        //     healthBar.value = currentHealth / maxHealth;
        Debug.Log("Health: " + currentHealth);
    }

    void Die()
    {
        Debug.Log("Player died!");
        // TODO: Respawn or show Game Over screen
    }
}
