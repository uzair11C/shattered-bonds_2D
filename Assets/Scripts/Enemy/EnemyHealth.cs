using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage! Remaining: {health}");
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        // Destroy(gameObject);
    }
}
