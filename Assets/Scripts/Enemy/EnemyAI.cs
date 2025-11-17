using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
    }

    private EnemyState currentState = EnemyState.Patrol;

    [Header("Detection")]
    [SerializeField]
    private Vector2 detectionBoxSize = new(5f, 2f);

    [SerializeField]
    private Vector2 detectionBoxOffset = new(2.5f, 0f);

    [SerializeField]
    private LayerMask playerLayer;

    [Header("Attack")]
    [SerializeField]
    private Vector2 attackBoxSize = new(1.5f, 1.5f);

    [SerializeField]
    private Vector2 attackBoxOffset = new(1f, 0f);

    [SerializeField]
    private float attackCooldown = 1f;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private float chaseSpeedMultiplier = 1.7f;

    [SerializeField]
    private float flipThreshold = 0.3f;

    [SerializeField]
    private float verticalFlipBlock = 0.5f;

    [Header("Patrol Points")]
    [SerializeField]
    private Transform pointA;

    [SerializeField]
    private Transform pointB;

    private Transform player;
    private Vector3 patrolTarget;
    private float lastAttackTime;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        patrolTarget = pointB.position;
    }

    private void Update()
    {
        DetectPlayer();

        switch (currentState)
        {
            case EnemyState.Patrol:
                UpdatePatrol();
                break;
            case EnemyState.Chase:
                UpdateChase();
                break;
            case EnemyState.Attack:
                UpdateAttack();
                break;
        }
    }

    // ---------------------------------------------------------
    //  DETECTION
    // ---------------------------------------------------------
    private void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapBox(
            (Vector2)transform.position + GetFacingOffset(detectionBoxOffset),
            detectionBoxSize,
            0f,
            playerLayer
        );

        if (hit)
        {
            player = hit.transform;
            if (currentState != EnemyState.Attack)
                currentState = EnemyState.Chase;
        }
        else
        {
            player = null;
            currentState = EnemyState.Patrol;
        }
    }

    // ---------------------------------------------------------
    // PATROL STATE
    // ---------------------------------------------------------
    private void UpdatePatrol()
    {
        FlipToward(patrolTarget.x);

        float patrolSpeed = moveSpeed * Time.deltaTime;

        Debug.Log("Patrolling toward point at speed: " + patrolSpeed);

        transform.position = Vector2.MoveTowards(transform.position, patrolTarget, patrolSpeed);

        if (Vector2.Distance(transform.position, patrolTarget) < 0.1f)
            patrolTarget = patrolTarget == pointA.position ? pointB.position : pointA.position;
    }

    // ---------------------------------------------------------
    // CHASE STATE
    // ---------------------------------------------------------
    private void UpdateChase()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        FlipToward(player.position.x);

        // Check if player is in attack box
        Collider2D attackHit = Physics2D.OverlapBox(
            (Vector2)transform.position + GetFacingOffset(attackBoxOffset),
            attackBoxSize,
            0f,
            playerLayer
        );

        if (attackHit)
        {
            currentState = EnemyState.Attack;
            return;
        }

        float chaseSpeed = moveSpeed * chaseSpeedMultiplier * Time.deltaTime;
        Debug.Log("Chasing player at speed: " + chaseSpeed);
        // Move toward player
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed);
    }

    // ---------------------------------------------------------
    // ATTACK STATE
    // ---------------------------------------------------------
    private void UpdateAttack()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        FlipToward(player.position.x);

        // Still in attack range?
        Collider2D hit = Physics2D.OverlapBox(
            (Vector2)transform.position + GetFacingOffset(attackBoxOffset),
            attackBoxSize,
            0f,
            playerLayer
        );

        if (hit == null)
        {
            currentState = EnemyState.Chase;
            return;
        }

        // Try attacking
        TryAttack(hit.GetComponent<PlayerHealth>());
    }

    private void TryAttack(PlayerHealth playerHealth)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator?.SetTrigger("AttackPlayer");
            lastAttackTime = Time.time;

            if (playerHealth != null)
                playerHealth.TakeDamage(10);
        }
    }

    // ---------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------
    private Vector2 GetFacingOffset(Vector2 offset)
    {
        return new Vector2(offset.x * Mathf.Sign(transform.localScale.x), offset.y);
    }

    private void FlipToward(float targetX)
    {
        float xDiff = targetX - transform.position.x;

        // Don't flip if player is above enemy (prevents flip-spam during jumps)
        if (player != null && player.position.y > transform.position.y + verticalFlipBlock)
            return;

        // Don't flip for tiny micro alignments
        if (Mathf.Abs(xDiff) < flipThreshold)
            return;

        bool shouldFlip =
            (xDiff > 0 && transform.localScale.x < 0) || (xDiff < 0 && transform.localScale.x > 0);

        if (shouldFlip)
            Flip();
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            (Vector2)transform.position + GetFacingOffset(detectionBoxOffset),
            detectionBoxSize
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            (Vector2)transform.position + GetFacingOffset(attackBoxOffset),
            attackBoxSize
        );
    }
}
