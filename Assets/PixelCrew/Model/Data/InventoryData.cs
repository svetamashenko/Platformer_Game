using PixelCrew.Model.Definitions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelCrew.Model.Data
{
    [Serializable]
    public class InventoryData
    {
        [SerializeField] private List<InventoryItemData> _inventory = new List<InventoryItemData>();

        public delegate void OnInventoryChanged(string id, int value);

        public OnInventoryChanged OnChanged;

        public void Add(string id, int value)
        {
            if (value <= 0) return;

            var itemDef = DefsFacade.I.Items.Get(id);
            if (itemDef.IsVoid) return;

            int currentCount = Count(id);

            if (itemDef.MaxStack > 0)
            {
                int availableSpace = itemDef.MaxStack - currentCount;
                if (availableSpace <= 0)
                {
                    return;
                }
                value = Mathf.Min(value, availableSpace);
            }

            if (itemDef.IsNonStackable)
            {
                var existingItems = _inventory.FindAll(item => item.Id == id);

                if (existingItems.Count == 1 && existingItems[0].Value == 0)
                {
                    existingItems[0].Value = 1;
                    OnChanged?.Invoke(id, 1);
                    return;
                }

                for (int i = 0; i < value; i++)
                {
                    var item = new InventoryItemData(id);
                    _inventory.Add(item);
                    OnChanged?.Invoke(id, 1);
                }
            }
            else
            {
                var item = GetItem(id);
                if (item == null)
                {
                    item = new InventoryItemData(id);
                    _inventory.Add(item);
                }
                item.Value += value;
                OnChanged?.Invoke(id, value);
            }
        }
        public InventoryData Clone()
        {
            var newInventory = new InventoryData();
            foreach (var item in _inventory)
            {
                newInventory._inventory.Add(new InventoryItemData(item.Id) { Value = item.Value });
            }
            return newInventory;
        }

        public void Reset(string id)
        {
            var item = GetItem(id);
            _inventory.Remove(item);
            OnChanged?.Invoke(id, 0);
        }

        public void Remove(string id, int value)
        {
            var itemDef = DefsFacade.I.Items.Get(id);
            if (itemDef.IsVoid || value <= 0) return;

            if (itemDef.IsNonStackable)
            {
                int removed = 0;
                for (int i = _inventory.Count - 1; i >= 0; i--)
                {
                    if (_inventory[i].Id == id && removed < value)
                    {
                        _inventory.RemoveAt(i);
                        removed++;
                        OnChanged?.Invoke(id, -1);
                    }
                }
            }
            else
            {
                var item = GetItem(id);
                if (item == null) return;

                item.Value -= value;
                if (item.Value <= 0)
                {
                    _inventory.Remove(item);
                    OnChanged?.Invoke(id, -value);
                }
                else
                {
                    OnChanged?.Invoke(id, -value);
                }
            }
        }

        private InventoryItemData GetItem(string id)
        {
            foreach (var itemData in _inventory)
            {
                if (itemData.Id == id)
                {
                    return itemData;
                }
            }
            return null;
        }

        internal int Count(string id)
        {
            var count = 0;
            foreach (var item in _inventory)
            {
                if (item.Id == id)
                {
                    count += item.Value;
                }
            }
            return count;
        }
    }

    [Serializable]
    public class InventoryItemData
    {
        [InventoryId] public string Id;
        public int Value;

        public InventoryItemData(string id)
        {
            Id = id;
            Value = 1;
        }
    }
}