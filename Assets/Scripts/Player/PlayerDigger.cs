using UnityEngine;
using Minecraft2D.UI;
using Minecraft2D.World;
using Minecraft2D.World.Resources;

namespace Minecraft2D.Player
{
    /// <summary>
    /// Игрок копает объекты ЛКМ.
    /// Raycast под курсор мыши → находит DamageableObject → наносит урон.
    /// При поломке спавнит дроп ресурсов и передаёт ему тип.
    /// </summary>
    public class PlayerDigger : MonoBehaviour
    {
        [Header("Настройки копания")]
        [SerializeField] private float damagePerSecond = 3f;
        [SerializeField] private float maxDigDistance = 5f;
        [SerializeField] private LayerMask interactableLayer;

        [Header("Ссылки")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private WorldProgressBar progressBar;

        [Header("Префабы дропа")]
        [SerializeField] private GameObject woodDropPrefab;
        [SerializeField] private GameObject stoneDropPrefab;

        private DamageableObject currentTarget;
        private float currentHealth01 = 1f;

        private void Update()
        {
            if (Input.GetMouseButton(0))
                HandleDigging();
            else
                StopDigging();
        }

        private void HandleDigging()
        {
            DamageableObject target = GetTargetUnderMouse();

            if (target == null || target.IsDestroyed)
            {
                StopDigging();
                return;
            }

            if (target != currentTarget)
                StartDigging(target);

            currentHealth01 -= (damagePerSecond / 10f) * Time.deltaTime;

            if (progressBar != null)
                progressBar.SetProgress(currentHealth01);

            bool destroyed = target.TakeDamage(damagePerSecond * Time.deltaTime);

            if (destroyed)
            {
                SpawnDrop(target);
                StopDigging();
                Destroy(target.gameObject);
            }
        }

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
        }

        private void StopDigging()
        {
            currentTarget = null;

            if (progressBar != null)
                progressBar.Hide();
        }

        private DamageableObject GetTargetUnderMouse()
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            float distance = Vector2.Distance(transform.position, mouseWorld);
            if (distance > maxDigDistance) return null;

            RaycastHit2D hit = Physics2D.Raycast(
                mouseWorld,
                Vector2.zero,
                Mathf.Infinity,
                interactableLayer
            );

            if (hit.collider == null) return null;

            return hit.collider.GetComponentInParent<DamageableObject>();
        }

        /// <summary>
        /// Спавнит дроп с правильным типом ресурса.
        /// После Instantiate находим ResourceItem и вызываем Setup().
        /// </summary>
        private void SpawnDrop(DamageableObject target)
        {
            GameObject prefab = target.DropResource switch
            {
                ResourceType.Wood => woodDropPrefab,
                ResourceType.Stone => stoneDropPrefab,
                _ => null
            };

            if (prefab == null)
            {
                Debug.LogWarning($"PlayerDigger: префаб для {target.DropResource} не задан!");
                return;
            }

            int amount = Random.Range(target.DropMinAmount, target.DropMaxAmount + 1);

            for (int i = 0; i < amount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * 0.5f;
                Vector3 pos = target.transform.position + (Vector3)offset;

                GameObject dropObj = Instantiate(prefab, pos, Quaternion.identity);

                ResourceItem item = dropObj.GetComponent<ResourceItem>();
                if (item != null)
                    item.Setup(target.DropResource, 1);
            }
        }
    }
}