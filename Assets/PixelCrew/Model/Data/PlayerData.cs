using System;
using UnityEngine;

namespace PixelCrew.Model.Data
{
    [Serializable]
    public class PlayerData
    {
        [SerializeField] private InventoryData _inventory;
        public int Hp;
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
                Hp = this.Hp,
                _inventory = this._inventory.Clone()
            };
        }
    }
}
