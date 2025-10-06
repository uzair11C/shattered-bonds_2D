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
    private float attackRate = 1f;

    private PlayerControls controls;
    private float nextAttackTime = 0f;
    private int comboStep = 0; // 0 = idle, 1 = Attack1, 2 = Attack2, 3 = Attack3

    private bool canCombo = false;
    private int currentAttack = 0;
    private bool isAttacking = false;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Attack.performed += Attack;
    }

    void OnDisable()
    {
        controls.Player.Attack.performed -= Attack;
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
                Debug.Log($"Combo Step: {comboStep}, Can Combo: {canCombo}");

                if (comboStep == 0)
                {
                    // Start first attack
                    Debug.Log("First Attack");
                    comboStep = 1;
                    animator.SetInteger("AttackIndex", comboStep);
                    animator.SetTrigger("Attack");
                    DoAttackHit();
                }
                else if (canCombo && comboStep < 3)
                {
                    // Queue next attack
                    Debug.Log("Combo Attack");
                    Debug.Log($"Current Combo Step: {comboStep}");
                    comboStep++;
                    animator.SetInteger("AttackIndex", comboStep);
                    animator.SetTrigger("Attack");
                    DoAttackHit();
                    canCombo = false;
                }

                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
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

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    // --- Animation Event Functions ---
    public void ComboWindow()
    {
        Debug.Log("Combo Window Opened");
        canCombo = true;
    }

    public void EndCombo()
    {
        Debug.Log("Combo Ended");
        canCombo = false;
        comboStep = 0; // Reset after combo chain ends
    }
}
