using UnityEngine;
using Minecraft2D.World.Resources;

namespace Minecraft2D.Crafting
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Minecraft2D/Crafting Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        [Header("Что крафтим")]
        public ResourceType result = ResourceType.Wall;
        public int resultAmount = 1;

        [Header("Из чего")]
        public ResourceType ingredient1 = ResourceType.Wood;
        public int ingredient1Amount = 3;
        public ResourceType ingredient2 = ResourceType.Stone;
        public int ingredient2Amount = 1;

        [Header("Описание")]
        public string displayName = "Стена";

        [Header("Требования")]
        public bool requiresWorkbench = false;
    }
}