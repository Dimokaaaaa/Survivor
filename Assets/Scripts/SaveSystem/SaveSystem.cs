using System.IO;
using UnityEngine;
using Minecraft2D.InventorySystem;
using Minecraft2D.World;
using Minecraft2D.World.Resources;

namespace Minecraft2D.Saving
{
    /// <summary>
    /// Сохранение / загрузка игры. F5 — save, F9 — load.
    /// Файл хранится в Application.persistentDataPath (работает в WebGL).
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        private const string FileName = "save.json";

        private string FilePath => Path.Combine(Application.persistentDataPath, FileName);

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5)) Save();
            if (Input.GetKeyDown(KeyCode.F9)) Load();
        }

        public void Save()
        {
            SaveData data = new SaveData();

            // Мир
            var spawner = FindObjectOfType<WorldSpawner>();
            data.worldSeed = spawner != null ? spawner.CurrentSeed : 0;

            // Игрок
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                data.playerX = player.transform.position.x;
                data.playerY = player.transform.position.y;
            }

            // Инвентарь
            var inv = Inventory.Instance;
            if (inv != null)
            {
                foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
                {
                    int amount = inv.Get(type);
                    if (amount > 0)
                        data.resources.Add(new ResourceEntry { type = (int)type, amount = amount });
                }
            }

            // Поставленные объекты
            foreach (var placer in FindObjectsOfType<PlacedObjectMarker>())
            {
                data.placedObjects.Add(new PlacedEntry
                {
                    type = (int)placer.Type,
                    x = placer.transform.position.x,
                    y = placer.transform.position.y,
                    rotation = placer.transform.eulerAngles.z
                });
            }

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
            Debug.Log($"Сохранено: {FilePath}");
        }

        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                Debug.LogWarning("Сохранение не найдено");
                return;
            }

            string json = File.ReadAllText(FilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // Мир — пересоздать с тем же seed
            var spawner = FindObjectOfType<WorldSpawner>();
            if (spawner != null && data.worldSeed != 0)
                spawner.RespawnWithSeed(data.worldSeed);

            // Игрок
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = new Vector3(data.playerX, data.playerY, 0f);

            // Инвентарь — очистить и залить заново
            var inv = Inventory.Instance;
            if (inv != null)
            {
                inv.Clear();
                foreach (var r in data.resources)
                    inv.Add((ResourceType)r.type, r.amount);
            }

            // Поставленные объекты — удалить все и поставить заново
            foreach (var p in FindObjectsOfType<PlacedObjectMarker>())
                Destroy(p.gameObject);

            var placer = FindObjectOfType<Player.PlayerPlacer>();
            if (placer != null)
            {
                foreach (var p in data.placedObjects)
                {
                    var prefab = placer.GetPrefabForType((ResourceType)p.type);
                    if (prefab == null) continue;
                    var obj = Instantiate(prefab, new Vector3(p.x, p.y, 0f), Quaternion.Euler(0, 0, p.rotation));
                    obj.AddComponent<PlacedObjectMarker>().Init((ResourceType)p.type);
                }
            }

            Debug.Log("Загружено");
        }
    }
}