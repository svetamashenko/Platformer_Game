using PixelCrew.Model.Definitions;
using PixelCrew.Creatures.Hero;
using UnityEngine;

namespace PixelCrew.Components.Inventory
{
    public class AddToInventoryComponent : MonoBehaviour
    {
        [InventoryId] [SerializeField] private string _id;
        [SerializeField] private int _count = 1;

        public void Add(GameObject go)
        {
            var hero = FindObjectOfType<Hero>();
            if (hero != null)
            {
                hero.AddToInventory(_id, _count);
            }
        }
    }
}