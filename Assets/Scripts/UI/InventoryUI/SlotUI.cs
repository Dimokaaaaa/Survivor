using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Minecraft2D.InventorySystem;

namespace Minecraft2D.UI
{
    /// <summary>
    /// UI одной ячейки инвентаря.
    /// </summary>
    public class SlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Button button;
        [SerializeField] private Image background;

        public int SlotIndex { get; private set; }

        public void Init(int index, Sprite icon, int count, bool isEmpty)
        {
            SlotIndex = index;

            if (isEmpty)
            {
                iconImage.enabled = false;
                countText.text = "";
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = icon;
                iconImage.color = Color.white;
                countText.text = count > 1 ? count.ToString() : "";
            }
        }

        public void SetSelected(bool selected)
        {
            if (background == null) return;
            background.color = selected
                ? new Color(1f, 1f, 0.3f, 0.5f)
                : new Color(0.2f, 0.2f, 0.2f, 0.85f);
        }

        public void SetClickHandler(System.Action<int> onClick)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(SlotIndex));
        }
    }
}