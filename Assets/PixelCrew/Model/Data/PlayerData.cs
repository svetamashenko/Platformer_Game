using Assets.PixelCrew.Model.Data.Properties;
using System;
using UnityEngine;

namespace PixelCrew.Model.Data
{
    [Serializable]
    public class PlayerData
    {
        [SerializeField] private InventoryData _inventory;
        public IntProperty Hp = new IntProperty();
        public InventoryData Inventory => _inventory;

        public void CopyFrom(PlayerData other)
        {
            Hp = other.Hp;
            _inventory = other._inventory.Clone();
        }

        public PlayerData Clone()
        {
            return new PlayerData
            {
                Hp = new IntProperty(this.Hp.Value),
                _inventory = this._inventory.Clone()
            };
        }
    }
}