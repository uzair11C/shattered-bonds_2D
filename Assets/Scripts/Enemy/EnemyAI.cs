using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection:")]
    [SerializeField]
    private Vector2 detectionBoxSize = new Vector2(5f, 2f);

    [SerializeField]
    private Vector2 detectionBoxOffset = new Vector2(2.5f, 0f);

    [SerializeField]
    private LayerMask playerLayer;

    [Header("Attack:")]
    [SerializeField]
    private Vector2 attackBoxSize = new(1.5f, 1.5f); // Vector2 simplified

    [SerializeField]
    private Vector2 attackBoxOffset = new(1f, 0f); // Vector2 simplified

    [SerializeField]
    private float attackCooldown = 1f;

    [SerializeField]
    private float moveSpeed = 2f;

    [Header("Patrol Points:")]
    [SerializeField]
    private Transform pointA;

    [SerializeField]
    private Transform pointB;

    private Transform player;
    private Vector3 target;
    private float lastAttackTime;
    private Animator enemyAnimator;

    private void Start()
    {
        target = pointB.position;
        enemyAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Step 1: Detect player
        Collider2D hit = Physics2D.OverlapBox(
            (Vector2)transform.position + detectionBoxOffset * Mathf.Sign(transform.localScale.x),
            detectionBoxSize,
            0f,
            playerLayer
        );

        if (hit)
        {
            player = hit.transform;
            ChaseAndAttack();
        }
        else
        {
            player = null;
            Patrol();
        }
    }

    private void Patrol()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            target = target == pointA.position ? pointB.position : pointA.position;
            Flip();
        }
    }

    private void ChaseAndAttack()
    {
        // Flip toward player
        if (
            (player.position.x > transform.position.x && transform.localScale.x < 0)
            || (player.position.x < transform.position.x && transform.localScale.x > 0)
        )
        {
            Flip();
        }

        // Step 2: Check attack box
        Collider2D attackHit = Physics2D.OverlapBox(
            (Vector2)transform.position + attackBoxOffset * Mathf.Sign(transform.localScale.x),
            attackBoxSize,
            0f,
            playerLayer
        );

        if (attackHit)
        {
            TryAttack(attackHit.GetComponent<PlayerHealth>());
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * 1.7f * Time.deltaTime
            );
        }
    }

    private void TryAttack(PlayerHealth playerHealth)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            enemyAnimator.SetTrigger("AttackPlayer");
            lastAttackTime = Time.time;
            if (playerHealth != null)
                playerHealth.TakeDamage(10f);
        }
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
            (Vector2)transform.position + detectionBoxOffset * Mathf.Sign(transform.localScale.x),
            detectionBoxSize
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            (Vector2)transform.position + attackBoxOffset * Mathf.Sign(transform.localScale.x),
            attackBoxSize
        );
    }
}
