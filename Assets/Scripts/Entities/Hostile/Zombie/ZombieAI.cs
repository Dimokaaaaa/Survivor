using UnityEngine;
using Minecraft2D.Player;

namespace Minecraft2D.Entities.Hostile.Zombie
{
    /// <summary>
    /// Простой ИИ зомби: идёт к игроку, если близко. Иначе стоит.
    /// Спрайт разворачивается через flipX (у нас только вид влево).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ZombieAI : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float detectionRadius = 8f;
        [SerializeField] private float attackRadius = 0.8f;
        [SerializeField] private int attackDamage = 10;
        [SerializeField] private float attackCooldown = 1f;

        private Rigidbody2D rb;
        private SpriteRenderer sr;
        private Transform player;
        private PlayerHealth playerHealth;
        private float lastAttackTime;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            }
        }

        private void FixedUpdate()
        {
            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance < attackRadius)
            {
                rb.velocity = Vector2.zero;
                TryAttack();
            }
            else if (distance < detectionRadius)
            {
                Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
                rb.velocity = dir * moveSpeed;

                // Спрайт только "влево". Если игрок справа → flipX = true.
                if (Mathf.Abs(dir.x) > 0.01f)
                    sr.flipX = dir.x > 0;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }

        private void TryAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown) return;
            lastAttackTime = Time.time;
            playerHealth?.TakeDamage(attackDamage);
        }
    }
}