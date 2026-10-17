using System.Collections;
using UnityEngine;

namespace Minecraft2D.World
{
    /// <summary>
    /// Позволяет объектам (деревьям, камням) исчезать и возрождаться через время.
    /// </summary>
    public class Respawnable : MonoBehaviour
    {
        [SerializeField] private float respawnTime = 60f; // 60 секунд

        private DamageableObject damageable;
        private SpriteRenderer[] renderers;
        private Collider2D[] colliders;

        private void Awake()
        {
            damageable = GetComponent<DamageableObject>();
            renderers = GetComponentsInChildren<SpriteRenderer>(true);
            colliders = GetComponentsInChildren<Collider2D>(true);

            if (damageable != null)
                damageable.OnDestroyed.AddListener(OnDestroyed);
        }

        private void OnDestroyed()
        {
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            // 1. Скрываем объект (выключаем рендер и физику)
            foreach (var r in renderers) r.enabled = false;
            foreach (var c in colliders) c.enabled = false;

            // 2. Ждём N секунд
            yield return new WaitForSeconds(respawnTime);

            // 3. Восстанавливаем здоровье и показываем
            if (damageable != null) damageable.ResetHealth();

            foreach (var r in renderers) r.enabled = true;
            foreach (var c in colliders) c.enabled = true;
        }
    }
}