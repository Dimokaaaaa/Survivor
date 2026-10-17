using UnityEngine;
using Minecraft2D.UI;
using Minecraft2D.World;
using Minecraft2D.World.Resources;

namespace Minecraft2D.Player
{
    /// <summary>
    /// Управляет копанием ресурсов и атакой по врагам.
    /// Ресурсы: зажать ЛКМ. Враги: клик ЛКМ (с кулдауном).
    /// </summary>
    public class PlayerDigger : MonoBehaviour
    {
        [Header("Настройки копания")]
        [SerializeField] private float baseDamagePerSecond = 3f;
        [SerializeField] private float maxDigDistance = 5f;
        [SerializeField] private LayerMask interactableLayer;

        [Header("Настройки боя")]
        [SerializeField] private float attackCooldown = 1f; // кулдаун между ударами
        [SerializeField] private float handDamage = 5f;     // урон без инструмента/не тем
        [SerializeField] private float swordDamage = 25f;   // урон мечом по врагам

        [Header("Бонусы инструментов (для копания)")]
        [SerializeField] private float toolBonusMultiplier = 2f;

        [Header("Ссылки")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private WorldProgressBar progressBar;
        [SerializeField] private HeldTool heldTool;

        [Header("Префабы дропа")]
        [SerializeField] private GameObject woodDropPrefab;
        [SerializeField] private GameObject stoneDropPrefab;

        private DamageableObject currentTarget;
        private float currentHealth01 = 1f;
        private float lastAttackTime;

        private void Update()
        {
            // Зажата ли ЛКМ
            if (Input.GetMouseButton(0))
                HandleInteraction();
            else
                StopDigging();
        }

        private void HandleInteraction()
        {
            DamageableObject target = GetTargetUnderMouse();

            if (target == null || target.IsDestroyed)
            {
                StopDigging();
                return;
            }

            if (target.isEnemy)
            {
                // ЛОГИКА БОЯ (клик + кулдаун)
                if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
                {
                    lastAttackTime = Time.time;
                    AttackEnemy(target);
                }
                StopDigging(); // Скрываем шкалу копания для врагов
            }
            else
            {
                // ЛОГИКА КОПАНИЯ (зажимаем ЛКМ)
                if (target != currentTarget)
                    StartDigging(target);

                DigResource(target);
            }
        }

        // ==========================================
        // БОЙ С ВРАГАМИ
        // ==========================================
        private void AttackEnemy(DamageableObject target)
        {
            float damage = handDamage;

            if (heldTool != null && heldTool.CurrentTool == ResourceType.Sword)
                damage = swordDamage;

            heldTool?.Swing(); // Анимация взмаха

            Debug.Log($"Игрок бьет {target.name} на {damage} урона");

            bool destroyed = target.TakeDamage(damage);
            if (destroyed)
            {
                SpawnDrop(target);
                Destroy(target.gameObject);
            }
        }

        // ==========================================
        // КОПАНИЕ РЕСУРСОВ
        // ==========================================
        private void DigResource(DamageableObject target)
        {
            float damage = GetDigDamage(target);

            currentHealth01 -= (damage / 10f) * Time.deltaTime;

            if (progressBar != null)
                progressBar.SetProgress(currentHealth01);

            bool destroyed = target.TakeDamage(damage * Time.deltaTime);

            if (destroyed)
            {
                SpawnDrop(target);
                StopDigging();

                // Если у объекта есть Respawnable (деревья, камни), не удаляем его
                if (target.GetComponent<Respawnable>() == null)
                    Destroy(target.gameObject);
            }
        }

        private float GetDigDamage(DamageableObject target)
        {
            float damage = baseDamagePerSecond;

            if (heldTool == null) return damage;

            ResourceType tool = heldTool.CurrentTool;

            // Кирка → бонус на камень
            if (tool == ResourceType.Pickaxe && target.DropResource == ResourceType.Stone)
                damage *= toolBonusMultiplier;

            // Топор → бонус на дерево
            if (tool == ResourceType.Axe && target.DropResource == ResourceType.Wood)
                damage *= toolBonusMultiplier;

            return damage;
        }

        // ==========================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // ==========================================
        private void StartDigging(DamageableObject target)
        {
            currentTarget = target;
            currentHealth01 = 1f;

            if (progressBar != null)
            {
                Vector3 pos = target.transform.position + new Vector3(0, 1.2f, 0);
                progressBar.Show(pos);
                progressBar.SetProgress(1f);
            }

            heldTool?.Swing();
        }

        private void StopDigging()
        {
            currentTarget = null;
            if (progressBar != null) progressBar.Hide();
        }

        private DamageableObject GetTargetUnderMouse()
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            float distance = Vector2.Distance(transform.position, mouseWorld);
            if (distance > maxDigDistance) return null;

            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero, Mathf.Infinity, interactableLayer);
            if (hit.collider == null) return null;

            return hit.collider.GetComponentInParent<DamageableObject>();
        }

        private void SpawnDrop(DamageableObject target)
        {
            GameObject prefab = target.DropResource switch
            {
                ResourceType.Wood => woodDropPrefab,
                ResourceType.Stone => stoneDropPrefab,
                _ => null // Для зомби и верстака дропа нет
            };

            if (prefab == null) return;

            int amount = Random.Range(target.DropMinAmount, target.DropMaxAmount + 1);

            for (int i = 0; i < amount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * 0.5f;
                Vector3 pos = target.transform.position + (Vector3)offset;

                GameObject dropObj = Instantiate(prefab, pos, Quaternion.identity);
                ResourceItem item = dropObj.GetComponent<ResourceItem>();
                if (item != null) item.Setup(target.DropResource, 1);
            }
        }
    }
}