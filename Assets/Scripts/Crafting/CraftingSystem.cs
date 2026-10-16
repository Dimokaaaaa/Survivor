using UnityEngine;
using Minecraft2D.InventorySystem;

namespace Minecraft2D.Crafting
{
    public class CraftingSystem : MonoBehaviour
    {
        public static CraftingSystem Instance { get; private set; }

        [SerializeField] private CraftingRecipe[] recipes;

        public CraftingRecipe[] Recipes => recipes;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public bool CanCraft(CraftingRecipe recipe)
        {
            var inv = Inventory.Instance;
            if (inv == null || recipe == null) return false;

            if (inv.Get(recipe.ingredient1) < recipe.ingredient1Amount) return false;
            if (recipe.ingredient2 != World.Resources.ResourceType.None &&
                inv.Get(recipe.ingredient2) < recipe.ingredient2Amount) return false;

            return true;
        }

        public bool Craft(CraftingRecipe recipe)
        {
            if (!CanCraft(recipe)) return false;

            var inv = Inventory.Instance;
            inv.Spend(recipe.ingredient1, recipe.ingredient1Amount);

            if (recipe.ingredient2 != World.Resources.ResourceType.None)
                inv.Spend(recipe.ingredient2, recipe.ingredient2Amount);

            inv.Add(recipe.result, recipe.resultAmount);
            return true;
        }
    }
}