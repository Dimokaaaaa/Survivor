using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Minecraft2D.Crafting;

namespace Minecraft2D.UI
{
    public class CraftingUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button craftWallButton;
        [SerializeField] private Button craftHouseButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI wallButtonText;
        [SerializeField] private TextMeshProUGUI houseButtonText;

        private void Start()
        {
            panel.SetActive(false);

            craftWallButton.onClick.AddListener(() => TryCraft(0, wallButtonText));
            craftHouseButton.onClick.AddListener(() => TryCraft(1, houseButtonText));
            closeButton.onClick.AddListener(() => panel.SetActive(false));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
                panel.SetActive(!panel.activeSelf);

            if (panel.activeSelf)
            {
                UpdateButtonStates();
            }
        }

        private void TryCraft(int index, TextMeshProUGUI btnText)
        {
            var system = CraftingSystem.Instance;
            if (system == null || index >= system.Recipes.Length) return;

            if (system.Craft(system.Recipes[index]))
                Debug.Log($"Скрафчено: {system.Recipes[index].displayName}");
            else
                Debug.Log("Недостаточно ресурсов");
        }

        private void UpdateButtonStates()
        {
            var system = CraftingSystem.Instance;
            if (system == null) return;

            if (system.Recipes.Length > 0 && wallButtonText != null)
            {
                var r = system.Recipes[0];
                wallButtonText.text = $"{r.displayName} ({r.ingredient1Amount} {r.ingredient1}, {r.ingredient2Amount} {r.ingredient2})";
            }
            if (system.Recipes.Length > 1 && houseButtonText != null)
            {
                var r = system.Recipes[1];
                houseButtonText.text = $"{r.displayName} ({r.ingredient1Amount} {r.ingredient1}, {r.ingredient2Amount} {r.ingredient2})";
            }
        }
    }
}