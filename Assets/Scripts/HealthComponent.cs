using UnityEngine;

public class HealthComponent : MonoBehaviour
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

    public void TakeDamage(int amount, Animator animator)
    {
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);

        if (animator != null)
            animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            healthBar.SetHealth(currentHealth);
            Die(animator);
        }
    }

    void Die(Animator animator)
    {
        Debug.Log("Player died!");
        if (animator != null)
            animator.SetTrigger("Die");
        // TODO: Respawn or show Game Over screen
    }
}
