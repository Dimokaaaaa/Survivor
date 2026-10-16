using UnityEngine;
using Minecraft2D.World.Resources;

namespace Minecraft2D.InventorySystem
{
    /// <summary>
    /// Хранит иконки для каждого ресурса.
    /// </summary>
    [CreateAssetMenu(fileName = "IconsDatabase", menuName = "Minecraft2D/Icons Database")]
    public class IconsDatabase : ScriptableObject
    {
        [System.Serializable]
        public class IconEntry
        {
            public ResourceType type;
            public Sprite icon;
        }

        [SerializeField] private IconEntry[] icons;

        public Sprite GetIcon(ResourceType type)
        {
            foreach (var entry in icons)
                if (entry.type == type) return entry.icon;
            return null;
        }
    }
}