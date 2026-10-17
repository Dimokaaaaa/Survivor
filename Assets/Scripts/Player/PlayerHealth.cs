using System;
using System.Collections;
using UnityEngine;

namespace Minecraft2D.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public static PlayerHealth Instance { get; private set; }

        [Header("Здоровье")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float invulnerabilityDuration = 0.5f;

        [Header("Регенерация")]
        [SerializeField] private bool enableRegen = true;
        [SerializeField] private float regenDelay = 5f;
        [SerializeField] private float regenPerSecond = 2f;
        [SerializeField] private float regenTickInterval = 0.5f;

        [Header("Отталкивание (Knockback)")]
        [SerializeField] private float knockbackForce = 8f;
        [SerializeField] private float knockbackDuration = 0.15f;
        [SerializeField] private Color flashColor = Color.red;

        public event Action<int, int> OnHealthChanged;
        public event Action OnPlayerDied;

        private int currentHealth;
        private float lastDamageTime;
        private float regenTimer;
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Coroutine flashCoroutine;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            currentHealth = maxHealth;

            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null) originalColor = spriteRenderer.color;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void Update()
        {
            if (!enableRegen) return;
            if (currentHealth <= 0) return;
            if (currentHealth >= maxHealth) return;
            if (Time.time - lastDamageTime < regenDelay) return;

            regenTimer -= Time.deltaTime;
            if (regenTimer <= 0f)
            {
                regenTimer = regenTickInterval;
                int heal = Mathf.CeilToInt(regenPerSecond * regenTickInterval);
                currentHealth = Mathf.Min(maxHealth, currentHealth + heal);
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
        }

        public void TakeDamage(int amount, Vector2 attackerPosition)
        {
            if (Time.time - lastDamageTime < invulnerabilityDuration) return;
            lastDamageTime = Time.time;
            regenTimer = regenTickInterval;

            currentHealth = Mathf.Max(0, currentHealth - amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Отталкивание
            if (rb != null)
            {
                Vector2 knockbackDir = ((Vector2)transform.position - attackerPosition).normalized;
                rb.velocity = knockbackDir * knockbackForce;
                StartCoroutine(ResetKnockback());
            }

            // Красная вспышка
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRed());

            if (currentHealth <= 0)
                Die();
        }

        private IEnumerator ResetKnockback()
        {
            yield return new WaitForSeconds(knockbackDuration);
            if (rb != null && currentHealth > 0)
                rb.velocity = Vector2.zero;
        }

        private IEnumerator FlashRed()
        {
            if (spriteRenderer == null) yield break;
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = originalColor;
        }

        private void Die()
        {
            OnPlayerDied?.Invoke();
        }

        public void Respawn()
        {
            currentHealth = maxHealth;
            transform.position = new Vector3(0.5f, 0.5f, 0f);
            lastDamageTime = 0f;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            if (spriteRenderer != null) spriteRenderer.color = originalColor;
        }
    }
}