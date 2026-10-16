using System.Collections.Generic;
using UnityEngine;

namespace Minecraft2D.World
{
    public class WorldSpawner : MonoBehaviour
    {
        [Header("Префабы")]
        [SerializeField] private GameObject treePrefab;
        [SerializeField] private GameObject rockPrefab;

        [Header("Количество")]
        [SerializeField] private int treeCount = 15;
        [SerializeField] private int rockCount = 12;

        [Header("Границы мира (в юнитах)")]
        [SerializeField] private Vector2 worldMin = new Vector2(-14f, -14f);
        [SerializeField] private Vector2 worldMax = new Vector2(14f, 14f);

        [Header("Безопасная зона вокруг игрока")]
        [SerializeField] private Vector2 playerSafeZoneCenter = new Vector2(0.5f, 0.5f);
        [SerializeField] private float playerSafeZoneRadius = 3f;

        [Header("Минимальная дистанция между объектами")]
        [SerializeField] private float minDistanceBetweenObjects = 2.5f;

        public int CurrentSeed { get; private set; }

        private readonly List<Vector2> usedPositions = new List<Vector2>();
        private readonly List<GameObject> spawnedObjects = new List<GameObject>();

        private void Start()
        {
            CurrentSeed = Random.Range(1, 1000000);
            SpawnAll(CurrentSeed);
        }

        public void RespawnWithSeed(int seed)
        {
            foreach (var obj in spawnedObjects)
                if (obj != null) Destroy(obj);

            spawnedObjects.Clear();
            usedPositions.Clear();

            CurrentSeed = seed;
            SpawnAll(seed);
        }

        private void SpawnAll(int seed)
        {
            Random.InitState(seed);

            SpawnObjects(treePrefab, treeCount);
            SpawnObjects(rockPrefab, rockCount);
        }

        private void SpawnObjects(GameObject prefab, int count)
        {
            if (prefab == null) return;

            int spawned = 0;
            int attempts = 0;
            int maxAttempts = count * 30;

            while (spawned < count && attempts < maxAttempts)
            {
                attempts++;
                Vector2 position = GetRandomPosition();

                if (!IsPositionFree(position)) continue;

                var obj = Instantiate(prefab, position, Quaternion.identity, transform);
                spawnedObjects.Add(obj);
                usedPositions.Add(position);
                spawned++;
            }
        }

        private Vector2 GetRandomPosition()
        {
            float x = Random.Range(worldMin.x, worldMax.x);
            float y = Random.Range(worldMin.y, worldMax.y);
            return new Vector2(Mathf.Floor(x) + 0.5f, Mathf.Floor(y) + 0.5f);
        }

        private bool IsPositionFree(Vector2 position)
        {
            if (Vector2.Distance(position, playerSafeZoneCenter) < playerSafeZoneRadius)
                return false;

            foreach (Vector2 used in usedPositions)
                if (Vector2.Distance(position, used) < minDistanceBetweenObjects)
                    return false;

            return true;
        }
    }
}