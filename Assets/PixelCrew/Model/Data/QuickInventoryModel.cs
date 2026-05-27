using Assets.PixelCrew.Model.Data.Properties;
using Assets.PixelCrew.Model.Definitions;
using Assets.PixelCrew.Utils.Disposables;
using PixelCrew.Model.Data;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.PixelCrew.Model.Data
{
    public class QuickInventoryModel : IDisposable
    {
        private PlayerData _data;

        private InventoryItemData[] _inventory;

        public InventoryItemData[] Inventory
        {
            get => _inventory;
            private set
            {
                _inventory = value;
                OnChanged?.Invoke();
            }
        }

        public readonly IntProperty SelectedIndex = new IntProperty();

        public event Action OnChanged;

        public InventoryItemData SelectedItem
        {
            get
            {
                if (Inventory == null || Inventory.Length == 0 || SelectedIndex.Value >= Inventory.Length)
                    return null;
                return Inventory[SelectedIndex.Value];
            }
        }

        public QuickInventoryModel(PlayerData data)
        {
            _data = data;
            RefreshInventory();
            _data.Inventory.OnChanged += OnChangedInventory;
        }

        private void RefreshInventory()
        {
            Inventory = _data.Inventory.GetAll(ItemTag.Usable);
            if (Inventory.Length == 0)
            {
                SelectedIndex.Value = 0;
            }
            else
            {
                SelectedIndex.Value = Mathf.Clamp(SelectedIndex.Value, 0, Inventory.Length - 1);
            }
        }

        public IDisposable Subscribe(Action call)
        {
            OnChanged += call;
            return new ActionDisposable(() => OnChanged -= call);
        }

        private void OnChangedInventory(string id, int value)
        {
            RefreshInventory();
        }

        public void SetNextItem()
        {
            if (Inventory.Length == 0) return;
            SelectedIndex.Value = (int)Mathf.Repeat(SelectedIndex.Value + 1, Inventory.Length);
        }

        public void Dispose()
        {
            _data.Inventory.OnChanged -= OnChangedInventory;
        }
    }
}