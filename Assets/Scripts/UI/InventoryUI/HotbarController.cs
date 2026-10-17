using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Minecraft2D.InventorySystem;

namespace Minecraft2D.UI
{
    /// <summary>
    /// ”правл€ет хотбаром (6 слотов) и полным инвентарЄм (48 слотов).
    ///  лавиши 1-6 Ч выбрать слот.  лавиша 0 Ч открыть/закрыть полный инвентарь.
    /// </summary>
    public class HotbarController : MonoBehaviour
    {
        [Header("’отбар (6 слотов)")]
        [SerializeField] private GameObject hotbarSlotPrefab;
        [SerializeField] private Transform hotbarParent;

        [Header("ѕолный инвентарь")]
        [SerializeField] private GameObject fullInventoryPanel;
        [SerializeField] private GameObject fullSlotPrefab;
        [SerializeField] private Transform fullInventoryParent;

        [Header(" нопка рюкзака")]
        [SerializeField] private Button backpackButton;

        [Header("«акрытие инвентар€")]
        [SerializeField] private Button closeInventoryButton;

        [Header("»конки")]
        [SerializeField] private IconsDatabase iconsDatabase;

        private const int HotbarSize = 6;
        private const int FullInventorySize = 48;

        private readonly List<HotbarSlotUI> hotbarViews = new();
        private readonly List<HotbarSlotUI> fullViews = new();

        public int SelectedSlot { get; private set; } = 0;

        private void Start()
        {
            BuildHotbar();
            BuildFullInventory();

            if (fullInventoryPanel != null)
                fullInventoryPanel.SetActive(false);

            if (backpackButton != null)
                backpackButton.onClick.AddListener(ToggleFullInventory);

            if (closeInventoryButton != null)
                closeInventoryButton.onClick.AddListener(ToggleFullInventory);

            if (Inventory.Instance != null)
                Inventory.Instance.OnInventoryChanged += RefreshAll;

            RefreshAll();
        }

        private void OnDestroy()
        {
            if (Inventory.Instance != null)
                Inventory.Instance.OnInventoryChanged -= RefreshAll;
        }

        private void Update()
        {
            //  лавиши 1-6 Ч выбор хотбара
            for (int i = 0; i < HotbarSize; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    SelectSlot(i);
            }

            //  лавиша 0 Ч открыть/закрыть полный инвентарь
            if (Input.GetKeyDown(KeyCode.Alpha0))
                ToggleFullInventory();
        }

        private void BuildHotbar()
        {
            foreach (Transform child in hotbarParent)
                Destroy(child.gameObject);

            hotbarViews.Clear();

            for (int i = 0; i < HotbarSize; i++)
            {
                var obj = Instantiate(hotbarSlotPrefab, hotbarParent);
                var view = obj.GetComponent<HotbarSlotUI>();
                view.Init(i);
                view.SetClickHandler(SelectSlot);
                hotbarViews.Add(view);
            }
        }

        private void BuildFullInventory()
        {
            foreach (Transform child in fullInventoryParent)
                Destroy(child.gameObject);

            fullViews.Clear();

            for (int i = 0; i < FullInventorySize; i++)
            {
                int globalIndex = HotbarSize + i;   // слоты 6..53
                var obj = Instantiate(fullSlotPrefab, fullInventoryParent);
                var view = obj.GetComponent<HotbarSlotUI>();
                view.Init(globalIndex);
                fullViews.Add(view);
            }
        }

        public void SelectSlot(int index)
        {
            if (index < 0 || index >= HotbarSize) return;
            SelectedSlot = index;
            RefreshSelection();
        }

        private void ToggleFullInventory()
        {
            if (fullInventoryPanel == null) return;
            fullInventoryPanel.SetActive(!fullInventoryPanel.activeSelf);
        }

        private void RefreshAll()
        {
            // ’отбар
            for (int i = 0; i < hotbarViews.Count; i++)
            {
                var slot = Inventory.Instance.GetSlot(i);
                if (slot.IsEmpty)
                    hotbarViews[i].SetContent(null, 0, true);
                else
                    hotbarViews[i].SetContent(iconsDatabase?.GetIcon(slot.type), slot.count, false);
            }

            // ѕолный инвентарь
            for (int i = 0; i < fullViews.Count; i++)
            {
                int globalIndex = HotbarSize + i;
                var slot = Inventory.Instance.GetSlot(globalIndex);
                if (slot.IsEmpty)
                    fullViews[i].SetContent(null, 0, true);
                else
                    fullViews[i].SetContent(iconsDatabase?.GetIcon(slot.type), slot.count, false);
            }

            RefreshSelection();
        }

        private void RefreshSelection()
        {
            for (int i = 0; i < hotbarViews.Count; i++)
                hotbarViews[i].SetSelected(i == SelectedSlot);
        }
    }
}