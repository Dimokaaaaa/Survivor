using UnityEngine;
using UnityEngine.Events;
using Minecraft2D.World.Resources;

namespace Minecraft2D.World
{
    /// <summary>
    /// Компонент для ломаемых объектов (дерево, камень).
    /// Хранит HP, принимает урон, сообщает об уничтожении.
    /// </summary>
    public class DamageableObject : MonoBehaviour
    {
        [Header("Прочность")]
        [SerializeField] private float maxHealth = 5f;

        [Header("Ресурсы при поломке")]
        [SerializeField] private ResourceType dropResource = ResourceType.Wood;
        [SerializeField] private int dropMinAmount = 1;
        [SerializeField] private int dropMaxAmount = 3;

        [Header("События")]
        public UnityEvent<float> OnDamaged;   // нормализованное HP (0..1)
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

        /// <summary>
        /// Наносит урон. Возвращает true, если объект уничтожен.
        /// </summary>
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
    }
}