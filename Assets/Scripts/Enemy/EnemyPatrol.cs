using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public Transform pointA,
        pointB;
    public Transform player;

    private Vector3 target;
    private float lastAttackTime;
    private bool playerInRange;
    private bool isChasing;

    void Start()
    {
        target = pointB.position;
    }

    void Update()
    {
        if (playerInRange && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= attackRange)
            {
                TryAttack();
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        isChasing = false;
        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            target = target == pointA.position ? pointB.position : pointA.position;
            Flip();
        }
    }

    void ChasePlayer()
    {
        isChasing = true;
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * 1.5f * Time.deltaTime
        );
        if (
            (player.position.x > transform.position.x && transform.localScale.x < 0)
            || (player.position.x < transform.position.x && transform.localScale.x > 0)
        )
        {
            Flip();
        }
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Enemy attacks!");
            lastAttackTime = Time.time;
            // Add attack animation or player.TakeDamage() here
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            player = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }
}
