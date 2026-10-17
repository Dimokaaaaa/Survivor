using UnityEngine;
using UnityEngine.Events;
using Minecraft2D.World.Resources;

namespace Minecraft2D.World
{
    public class DamageableObject : MonoBehaviour
    {
        [Header("Прочность")]
        [SerializeField] private float maxHealth = 5f;

        [Header("Тип объекта")]
        public bool isEnemy = false; // ✅ Галка для зомби

        [Header("Ресурсы при поломке")]
        [SerializeField] private ResourceType dropResource = ResourceType.Wood;
        [SerializeField] private int dropMinAmount = 1;
        [SerializeField] private int dropMaxAmount = 3;

        [Header("События")]
        public UnityEvent<float> OnDamaged;
        public UnityEvent OnDestroyed;

        private float currentHealth;

        public bool IsDestroyed { get; private set; }
        public ResourceType DropResource => dropResource;
        public int DropMinAmount => dropMinAmount;
        public int DropMaxAmount => dropMaxAmount;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public bool TakeDamage(float amount)
        {
            if (IsDestroyed) return false;

            currentHealth -= amount;
            float normalizedHp = Mathf.Clamp01(currentHealth / maxHealth);

            OnDamaged?.Invoke(normalizedHp);

            if (currentHealth <= 0f)
            {
                IsDestroyed = true;
                OnDestroyed?.Invoke();
                return true;
            }
            return false;
        }

        // ✅ ДОБАВЬ ЭТОТ МЕТОД ДЛЯ ВОЗРОЖДЕНИЯ РЕСУРСОВ
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            IsDestroyed = false;
            OnDamaged?.Invoke(1f); // Восстанавливаем шкалу (если есть)
        }
    }
}