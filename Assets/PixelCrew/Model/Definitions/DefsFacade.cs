using Assets.PixelCrew.Model.Definitions;
using UnityEngine;

namespace PixelCrew.Model.Definitions
{
    [CreateAssetMenu(menuName ="Defs/DefsFacade", fileName = "DefsFacade")]
    public class DefsFacade : ScriptableObject
    {
        [SerializeField] private InventoryItemsDef _items;
        public InventoryItemsDef Items => _items;
        public PlayerDefs Player;

        private static DefsFacade _instance;
        public static DefsFacade I => _instance == null ? LoadDefs() : _instance;

        private static DefsFacade LoadDefs()
        {
            Resources.Load<InventoryItemsDef>("InventoryItems");
            return _instance = Resources.Load<DefsFacade>("DefsFacade");
        }
    }
}