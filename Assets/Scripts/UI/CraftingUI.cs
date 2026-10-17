using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Minecraft2D.Crafting;

namespace Minecraft2D.UI
{
    public class CraftingUI : MonoBehaviour
    {
        public static CraftingUI Instance { get; private set; }

        [Header("UI")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Transform recipeContent;
        [SerializeField] private GameObject recipeButtonPrefab;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject workbenchHint;

        [Header("Цвета кнопок")]
        [SerializeField] private Color canCraftColor = new Color(0.6f, 1f, 0.6f, 1f);
        [SerializeField] private Color cannotCraftColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        [SerializeField] private Color needsWorkbenchColor = new Color(1f, 0.7f, 0.4f, 1f);

        private readonly List<Button> recipeButtons = new();
        private readonly List<TextMeshProUGUI> recipeTexts = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            panel.SetActive(false);
            if (workbenchHint != null) workbenchHint.SetActive(false);

            closeButton.onClick.AddListener(() => panel.SetActive(false));

            BuildRecipeButtons();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
                panel.SetActive(!panel.activeSelf);

            if (panel.activeSelf)
                RefreshRecipeStates();
        }

        // === ПУБЛИЧНЫЕ МЕТОДЫ (для WorkbenchInteraction) ===

        public void ShowWorkbenchHint(bool show)
        {
            if (workbenchHint != null) workbenchHint.SetActive(show);
        }

        public void ToggleCraftingNearWorkbench()
        {
            panel.SetActive(!panel.activeSelf);
            if (panel.activeSelf) RefreshRecipeStates();
        }

        // === ПОСТРОЕНИЕ КНОПОК ===

        private void BuildRecipeButtons()
        {
            // Очищаем
            foreach (Transform child in recipeContent)
                Destroy(child.gameObject);

            recipeButtons.Clear();
            recipeTexts.Clear();

            var system = CraftingSystem.Instance;
            if (system == null) return;

            for (int i = 0; i < system.Recipes.Length; i++)
            {
                int index = i;
                var r = system.Recipes[i];

                var obj = Instantiate(recipeButtonPrefab, recipeContent);
                var btn = obj.GetComponent<Button>();
                var txt = obj.GetComponentInChildren<TextMeshProUGUI>();

                btn.onClick.AddListener(() => TryCraft(index));

                recipeButtons.Add(btn);
                recipeTexts.Add(txt);
            }

            RefreshRecipeStates();
        }

        // === ОБНОВЛЕНИЕ СОСТОЯНИЯ ===

        private void RefreshRecipeStates()
        {
            var system = CraftingSystem.Instance;
            if (system == null) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            bool nearWorkbench = player != null &&
                World.WorkbenchInteraction.IsPlayerNearAnyWorkbench(player.transform.position);

            for (int i = 0; i < recipeButtons.Count && i < system.Recipes.Length; i++)
            {
                var r = system.Recipes[i];
                var btn = recipeButtons[i];
                var txt = recipeTexts[i];

                bool needsWb = r.requiresWorkbench && !nearWorkbench;

                // Текст
                if (txt != null)
                {
                    string suffix = needsWb ? "  [нужен верстак]" : "";
                    txt.text = $"{r.displayName}  ({r.ingredient1Amount} {r.ingredient1}" +
                               (r.ingredient2 != World.Resources.ResourceType.None
                                    ? $", {r.ingredient2Amount} {r.ingredient2}"
                                    : "") + $"){suffix}";
                }

                // Цвет и доступность
                var img = btn.GetComponent<Image>();
                if (img != null)
                {
                    if (needsWb)
                        img.color = needsWorkbenchColor;
                    else if (system.CanCraft(r))
                        img.color = canCraftColor;
                    else
                        img.color = cannotCraftColor;
                }

                btn.interactable = system.CanCraft(r);
            }
        }

        // === КРАФТ ===

        private void TryCraft(int index)
        {
            var system = CraftingSystem.Instance;
            if (system == null || index >= system.Recipes.Length) return;

            var recipe = system.Recipes[index];

            if (recipe.requiresWorkbench)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null && !World.WorkbenchInteraction.IsPlayerNearAnyWorkbench(player.transform.position))
                {
                    Debug.Log($"Для {recipe.displayName} нужен верстак рядом!");
                    return;
                }
            }

            if (system.Craft(recipe))
                Debug.Log($"Скрафчено: {recipe.displayName}");
            else
                Debug.Log("Недостаточно ресурсов");

            RefreshRecipeStates();
        }
    }
}