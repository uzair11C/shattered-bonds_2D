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

    [Header("Damage Settings")]
    [SerializeField]
    private int attack1Damage = 8;

    [SerializeField]
    private int attack2Damage = 10;

    [SerializeField]
    private int attack3Damage = 14;

    private PlayerControls controls;
    private float lastAttackTime = 0f;
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
            if (animator.GetBool("Attack1") == true)
            {
                animator.SetBool("Attack1", false);
            }
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
        int damage = 0;
        switch (pressCount)
        {
            case 1:
                damage = attack1Damage;
                break;
            case 2:
                damage = attack2Damage;
                break;
            case 3:
                damage = attack3Damage;
                break;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );
        foreach (var enemy in hitEnemies)
        {
            Debug.Log($"Hit {enemy.name} for {damage} damage");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
