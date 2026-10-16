using System;
using UnityEngine;

namespace Minecraft2D.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public static PlayerHealth Instance { get; private set; }

        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float invulnerabilityDuration = 0.5f;

        public event Action<int, int> OnHealthChanged;
        public event Action OnPlayerDied;

        private int currentHealth;
        private float lastDamageTime;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            currentHealth = maxHealth;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (Time.time - lastDamageTime < invulnerabilityDuration) return;
            lastDamageTime = Time.time;

            currentHealth = Mathf.Max(0, currentHealth - amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
                Die();
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
        }
    }
}