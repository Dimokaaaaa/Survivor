using UnityEngine;
using Minecraft2D.InventorySystem;
using Minecraft2D.World;
using Minecraft2D.World.Resources;
using Minecraft2D.UI;

namespace Minecraft2D.Player
{
    /// <summary>
    /// Размещение построек. Активируется клавишей F.
    /// Выбор постройки — клавиши 1/2. Поворот — R (только для стены).
    /// При постановке добавляет PlacedObjectMarker — для сохранения.
    /// </summary>
    public class PlayerPlacer : MonoBehaviour
    {
        public static PlayerPlacer Instance { get; private set; }

        [Header("Префабы")]
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject housePrefab;
        [SerializeField] private GameObject workbenchPrefab;

        [Header("Настройки")]
        [SerializeField] private float maxPlaceDistance = 6f;
        [SerializeField] private Color ghostValidColor = new Color(1f, 1f, 1f, 0.5f);

        [Header("Смещения по типу (юниты)")]
        [SerializeField] private Vector2 wallOffset = new Vector2(0f, -0.5f);
        [SerializeField] private Vector2 houseOffset = Vector2.zero;
        [SerializeField] private Vector2 workbenchOffset = Vector2.zero;

        [Header("Ссылки")]
        [SerializeField] private Camera mainCamera;

        public bool IsBuildMode { get; private set; }
        public ResourceType SelectedType { get; private set; } = ResourceType.Wall;
        public float CurrentRotation { get; private set; }

        private GameObject ghostObject;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            SetBuildMode(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
                SetBuildMode(!IsBuildMode);

            if (!IsBuildMode) return;

            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectType(ResourceType.Wall);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectType(ResourceType.House);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectType(ResourceType.Workbench);

            // R работает ТОЛЬКО для стены
            if (Input.GetKeyDown(KeyCode.R) && SelectedType == ResourceType.Wall)
                RotateGhost();

            UpdateGhost();

            if (Input.GetMouseButtonDown(1))
                TryPlace();
        }

        public void SetBuildMode(bool enabled)
        {
            IsBuildMode = enabled;

            if (enabled) RecreateGhost();
            else if (ghostObject != null) Destroy(ghostObject);

            BuildingUI.Instance?.OnBuildModeChanged(enabled);
        }

        public void SelectType(ResourceType type)
        {
            SelectedType = type;

            // Сброс поворота для дома — всегда 0°
            if (type == ResourceType.House)
                CurrentRotation = 0f;

            if (IsBuildMode) RecreateGhost();
            BuildingUI.Instance?.OnSelectionChanged(type);
        }

        private void RotateGhost()
        {
            CurrentRotation = (CurrentRotation + 90f) % 360f;
            if (ghostObject != null)
                ghostObject.transform.rotation = Quaternion.Euler(0, 0, CurrentRotation);
        }

        private void RecreateGhost()
        {
            if (ghostObject != null) Destroy(ghostObject);

            GameObject prefab = GetPrefabForType(SelectedType);
            if (prefab == null) return;

            ghostObject = Instantiate(prefab);
            Destroy(ghostObject.GetComponent<Collider2D>());

            var renderer = ghostObject.GetComponentInChildren<SpriteRenderer>();
            if (renderer != null) renderer.color = ghostValidColor;
        }

        /// <summary>
        /// Публичный — используется SaveSystem при загрузке.
        /// </summary>
        public GameObject GetPrefabForType(ResourceType type)
        {
            return type switch
            {
                ResourceType.Wall => wallPrefab,
                ResourceType.House => housePrefab,
                ResourceType.Workbench => workbenchPrefab,
                _ => null
            };
        }

        private Vector3 GetSnappedMousePosition()
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            float x = Mathf.Floor(mouseWorld.x) + 0.5f;
            float y = Mathf.Floor(mouseWorld.y) + 0.5f;

            Vector2 offset = SelectedType switch
            {
                ResourceType.Wall => wallOffset,
                ResourceType.House => houseOffset,
                ResourceType.Workbench => workbenchOffset,
                _ => Vector2.zero
            };

            return new Vector3(x + offset.x, y + offset.y, 0f);
        }

        private void UpdateGhost()
        {
            if (ghostObject == null) return;

            var inv = Inventory.Instance;
            bool hasResources = inv != null && inv.Get(SelectedType) > 0;

            ghostObject.SetActive(hasResources);
            if (hasResources)
            {
                ghostObject.transform.position = GetSnappedMousePosition();
                ghostObject.transform.rotation = Quaternion.Euler(0, 0, CurrentRotation);
            }
        }

        private void TryPlace()
        {
            var inv = Inventory.Instance;
            if (inv == null || inv.Get(SelectedType) <= 0) return;

            Vector3 pos = GetSnappedMousePosition();
            if (Vector2.Distance(transform.position, pos) > maxPlaceDistance) return;

            GameObject prefab = GetPrefabForType(SelectedType);
            if (prefab == null) return;

            GameObject placed = Instantiate(prefab, pos, Quaternion.identity);
            placed.transform.rotation = Quaternion.Euler(0, 0, CurrentRotation);

            // Маркер для сохранения
            var marker = placed.AddComponent<PlacedObjectMarker>();
            marker.Init(SelectedType);

            inv.Spend(SelectedType, 1);
        }

        private void OnDestroy()
        {
            if (ghostObject != null) Destroy(ghostObject);
            if (Instance == this) Instance = null;
        }
    }
}