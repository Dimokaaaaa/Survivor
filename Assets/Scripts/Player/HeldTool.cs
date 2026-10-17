using UnityEngine;
using Minecraft2D.InventorySystem;
using Minecraft2D.UI;
using Minecraft2D.World.Resources;

namespace Minecraft2D.Player
{
    /// <summary>
    /// Инструмент, отображаемый рядом с игроком.
    /// Меняется в зависимости от выбранного слота хотбара.
    /// Крутится при Swing.
    /// </summary>
    public class HeldTool : MonoBehaviour
    {
        [Header("Ссылки")]
        [SerializeField] private SpriteRenderer toolRenderer;
        [SerializeField] private HotbarController hotbar;
        [SerializeField] private IconsDatabase iconsDatabase;

        [Header("Позиция инструмента")]
        [SerializeField] private Vector3 offset = new Vector3(0.5f, 0.3f, 0f);

        [Header("Анимация Swing")]
        [SerializeField] private float swingDuration = 0.3f;
        [SerializeField] private float swingRotation = 360f;

        public ResourceType CurrentTool { get; private set; } = ResourceType.None;

        private float swingTimer;
        private bool isSwinging;

        private void Start()
        {
            if (hotbar == null)
                hotbar = FindObjectOfType<HotbarController>();

            transform.localPosition = offset;
            UpdateToolSprite();
        }

        private void Update()
        {
            UpdateToolSprite();

            if (isSwinging)
            {
                swingTimer -= Time.deltaTime;
                float t = 1f - (swingTimer / swingDuration);

                // Вращение вокруг Z
                float angle = Mathf.Lerp(0f, swingRotation, t);
                transform.localRotation = Quaternion.Euler(0, 0, angle);

                if (swingTimer <= 0f)
                {
                    isSwinging = false;
                    transform.localRotation = Quaternion.identity;
                }
            }
        }

        private void UpdateToolSprite()
        {
            if (hotbar == null || Inventory.Instance == null || toolRenderer == null) return;

            var slot = Inventory.Instance.GetSlot(hotbar.SelectedSlot);

            ResourceType newTool = slot.IsEmpty ? ResourceType.None : slot.type;

            // Только инструменты отображаем
            bool isTool = newTool == ResourceType.Pickaxe ||
                          newTool == ResourceType.Sword ||
                          newTool == ResourceType.Axe;

            if (!isTool) newTool = ResourceType.None;

            if (newTool == CurrentTool) return;

            CurrentTool = newTool;

            if (CurrentTool == ResourceType.None)
            {
                toolRenderer.enabled = false;
            }
            else
            {
                toolRenderer.enabled = true;
                Sprite sprite = iconsDatabase != null ? iconsDatabase.GetIcon(CurrentTool) : null;
                if (sprite != null) toolRenderer.sprite = sprite;
            }
        }

        public void Swing()
        {
            if (CurrentTool == ResourceType.None) return;

            isSwinging = true;
            swingTimer = swingDuration;
        }

        private void LateUpdate()
        {
            // Инструмент не должен вращаться вместе с игроком
            transform.rotation = Quaternion.identity;
        }
    }
}