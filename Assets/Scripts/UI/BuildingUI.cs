using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Minecraft2D.InventorySystem;
using Minecraft2D.Player;
using Minecraft2D.World.Resources;

namespace Minecraft2D.UI
{
    public class BuildingUI : MonoBehaviour
    {
        public static BuildingUI Instance { get; private set; }

        [System.Serializable]
        public class Slot
        {
            public ResourceType type;
            public Button button;
            public Image iconImage;
            public TextMeshProUGUI countText;
            public TextMeshProUGUI hotkeyText;
        }

        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Slot[] slots;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            panelRoot.SetActive(false);

            for (int i = 0; i < slots.Length; i++)
            {
                int index = i;
                slots[i].button.onClick.AddListener(() => OnSlotClicked(index));
                slots[i].hotkeyText.text = (i + 1).ToString();
            }

            if (Inventory.Instance != null)
                Inventory.Instance.OnInventoryChanged += RefreshAll;
        }

        private void OnDestroy()
        {
            if (Inventory.Instance != null)
                Inventory.Instance.OnInventoryChanged -= RefreshAll;
            if (Instance == this) Instance = null;
        }

        public void OnBuildModeChanged(bool enabled)
        {
            panelRoot.SetActive(enabled);
            if (enabled) RefreshAll();
        }

        public void OnSelectionChanged(ResourceType selected)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                var outline = slots[i].button.GetComponent<Outline>();
                if (outline == null) outline = slots[i].button.gameObject.AddComponent<Outline>();
                outline.effectColor = slots[i].type == selected ? Color.yellow : new Color(0, 0, 0, 0);
                outline.effectDistance = new Vector2(3, -3);
            }
        }

        private void RefreshAll()
        {
            foreach (var slot in slots)
            {
                int count = Inventory.Instance != null ? Inventory.Instance.Get(slot.type) : 0;
                slot.countText.text = count.ToString();
                slot.iconImage.color = count > 0 ? Color.white : new Color(1f, 1f, 1f, 0.3f);
            }
        }

        private void OnSlotClicked(int index)
        {
            if (index < 0 || index >= slots.Length) return;
            PlayerPlacer.Instance?.SelectType(slots[index].type);
        }
    }
}