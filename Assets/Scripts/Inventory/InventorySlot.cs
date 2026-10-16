using System;
using Minecraft2D.World.Resources;

namespace Minecraft2D.InventorySystem
{
    /// <summary>
    /// Один слот инвентаря. Хранит тип ресурса и количество.
    /// </summary>
    [Serializable]
    public class InventorySlot
    {
        public ResourceType type = ResourceType.None;
        public int count = 0;

        public bool IsEmpty => type == ResourceType.None || count <= 0;

        public void Clear()
        {
            type = ResourceType.None;
            count = 0;
        }

        public bool CanStack(ResourceType otherType, int maxStack)
        {
            return !IsEmpty && type == otherType && count < maxStack;
        }
    }
}