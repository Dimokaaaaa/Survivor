using System;
using System.Collections.Generic;
using UnityEngine;
using Minecraft2D.World.Resources;

namespace Minecraft2D.InventorySystem
{
    [DefaultExecutionOrder(-100)]
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }

        public event Action OnInventoryChanged;

        [Header("Настройки")]
        [SerializeField] private int slotCount = 54;   // 6 хотбар + 48 полный
        [SerializeField] private int maxStackSize = 99;

        private InventorySlot[] slots;

        public int SlotCount => slotCount;
        public int MaxStackSize => maxStackSize;
        public InventorySlot GetSlot(int index) => slots[index];

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            slots = new InventorySlot[slotCount];
            for (int i = 0; i < slotCount; i++)
                slots[i] = new InventorySlot();
        }

        /// <summary>Добавить ресурс. Стек автоматически.</summary>
        public void Add(ResourceType type, int amount)
        {
            if (amount <= 0 || type == ResourceType.None) return;

            // Сначала в существующий стек
            foreach (var slot in slots)
            {
                if (slot.CanStack(type, maxStackSize))
                {
                    int toAdd = Mathf.Min(amount, maxStackSize - slot.count);
                    slot.count += toAdd;
                    amount -= toAdd;
                    if (amount <= 0) { NotifyChanged(); return; }
                }
            }

            // Потом в свободные
            foreach (var slot in slots)
            {
                if (slot.IsEmpty)
                {
                    slot.type = type;
                    slot.count = Mathf.Min(amount, maxStackSize);
                    amount -= slot.count;
                    if (amount <= 0) { NotifyChanged(); return; }
                }
            }

            if (amount > 0)
                Debug.LogWarning($"Инвентарь полон! Не вместилось {amount} {type}");

            NotifyChanged();
        }

        /// <summary>Сколько всего ресурса в инвентаре.</summary>
        public int Get(ResourceType type)
        {
            int total = 0;
            foreach (var slot in slots)
                if (slot.type == type) total += slot.count;
            return total;
        }

        /// <summary>Потратить ресурс. Возвращает false, если не хватает.</summary>
        public bool Spend(ResourceType type, int amount)
        {
            if (Get(type) < amount) return false;

            int remaining = amount;
            for (int i = slots.Length - 1; i >= 0; i--)
            {
                if (slots[i].type != type) continue;

                int take = Mathf.Min(remaining, slots[i].count);
                slots[i].count -= take;
                remaining -= take;

                if (slots[i].count <= 0) slots[i].Clear();
                if (remaining <= 0) break;
            }

            NotifyChanged();
            return true;
        }

        public void Clear()
        {
            foreach (var slot in slots) slot.Clear();
            NotifyChanged();
        }

        /// <summary>Обменять два слота местами (для drag&drop).</summary>
        public void SwapSlots(int a, int b)
        {
            if (a == b) return;
            if (a < 0 || a >= slotCount || b < 0 || b >= slotCount) return;

            (slots[a], slots[b]) = (slots[b], slots[a]);
            NotifyChanged();
        }

        private void NotifyChanged()
        {
            OnInventoryChanged?.Invoke();
        }
    }
}