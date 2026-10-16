using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft2D.World
{
    public class ZombieSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject zombiePrefab;

        [Header("День")]
        [SerializeField] private int dayMaxZombies = 2;
        [SerializeField] private float daySpawnInterval = 12f;

        [Header("Ночь")]
        [SerializeField] private int nightMaxZombies = 10;
        [SerializeField] private float nightSpawnInterval = 3f;

        [SerializeField] private float spawnRadius = 12f;

        private readonly List<GameObject> aliveZombies = new();
        private int currentMaxZombies;
        private float currentInterval;

        private void Start()
        {
            currentMaxZombies = dayMaxZombies;
            currentInterval = daySpawnInterval;

            if (DayNightCycle.Instance != null)
                DayNightCycle.Instance.OnDayNightChanged += OnDayNightChanged;

            StartCoroutine(SpawnLoop());
        }

        private void OnDestroy()
        {
            if (DayNightCycle.Instance != null)
                DayNightCycle.Instance.OnDayNightChanged -= OnDayNightChanged;
        }

        private void OnDayNightChanged(bool isDay)
        {
            currentMaxZombies = isDay ? dayMaxZombies : nightMaxZombies;
            currentInterval = isDay ? daySpawnInterval : nightSpawnInterval;
            Debug.Log($"ZombieSpawner: день={isDay}, max={currentMaxZombies}, interval={currentInterval}");
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(currentInterval);
                TrySpawn();
            }
        }

        private void TrySpawn()
        {
            aliveZombies.RemoveAll(z => z == null);
            if (aliveZombies.Count >= currentMaxZombies) return;
            if (zombiePrefab == null) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            Vector2 randomDir = Random.insideUnitCircle.normalized * spawnRadius;
            Vector2 spawnPos = (Vector2)player.transform.position + randomDir;
            spawnPos = new Vector2(Mathf.Floor(spawnPos.x) + 0.5f, Mathf.Floor(spawnPos.y) + 0.5f);

            var zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity, transform);
            aliveZombies.Add(zombie);
        }
    }
}