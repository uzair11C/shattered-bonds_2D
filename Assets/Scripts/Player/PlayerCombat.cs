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

    [Header("Combo Settings")]
    [SerializeField]
    private float attackRate = 2f;

    [SerializeField]
    private float comboResetTime = 1f;

    private PlayerControls controls;
    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private int pressCount = 0;

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

    private void Update()
    {
        // Reset combo if time since last attack too long
        if (Time.time - lastAttackTime > comboResetTime)
        {
            // comboStep = 0;
            // animator.SetInteger("ComboStep", 0);
            pressCount = 0;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack input received");
        if (context.performed)
        {
            Debug.Log("Performing attack");
            lastAttackTime = Time.time;
            pressCount++;
            if (pressCount == 1)
            {
                animator.SetBool("Attack1", true);
            }

            pressCount = Mathf.Clamp(pressCount, 0, 3);

            DoAttackHit();
        }
    }

    public void StopAttack1()
    {
        if (pressCount >= 2)
        {
            animator.SetBool("Attack2", true);
        }
        else
        {
            animator.SetBool("Attack1", false);
            pressCount = 0;
        }
    }

    public void StopAttack2()
    {
        if (pressCount >= 3)
        {
            animator.SetBool("Attack3", true);
        }
        else
        {
            animator.SetBool("Attack2", false);
            pressCount = 0;
        }
    }

    public void StopAttack3()
    {
        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
        animator.SetBool("Attack3", false);
        pressCount = 0;
    }

    private void DoAttackHit()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );
        foreach (var enemy in hitEnemies)
        {
            Debug.Log($"Hit {enemy.name}");
        }
    }

    // === These will be called by Animation Events ===
    public void ComboAttackEnd()
    {
        Debug.Log("Combo attack ended");
        isAttacking = false;
    }

    public void ResetCombo()
    {
        Debug.Log("Resetting combo");
        comboStep = 0;
        animator.SetInteger("ComboStep", 0);
        // animator.ResetTrigger("Attack");
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
