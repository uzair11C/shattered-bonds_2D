using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private float attackRange = 0.5f;

    [SerializeField]
    private LayerMask enemyLayers;

    [SerializeField]
    private float attackRate = 2f;

    private PlayerControls controls;
    private float nextAttackTime = 0f;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update() { }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Time.time >= nextAttackTime)
            {
                Debug.Log("Melee Attack");
                // Play attack animation
                animator.SetTrigger("Attack");

                // detect enemies in range
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                    attackPoint.position,
                    attackRange,
                    enemyLayers
                );
                // do damage
                foreach (Collider2D enemy in hitEnemies)
                {
                    Debug.Log("We hit " + enemy.name);
                }

                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
