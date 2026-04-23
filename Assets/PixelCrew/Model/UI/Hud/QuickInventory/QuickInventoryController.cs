using Assets.PixelCrew.Utils.Disposables;
using PixelCrew.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PixelCrew.Model.UI.Hud.QuickInventory
{
    public class QuickInventoryController : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private InventoryItemWidget _prefab;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private GameSession _session;
        private List<InventoryItemWidget> _createdItems = new List<InventoryItemWidget>();

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            _trash.Retain(_session.QuickInventory.Subscribe(Rebuild));
            Rebuild();
        }

        private void Rebuild()
        {
                if (this == null || gameObject == null || !gameObject.activeInHierarchy)
        return;

            var _inventory = _session.QuickInventory.Inventory;
             
            for (int i = _createdItems.Count; i<_inventory.Length; i++)
            {
                var item = Instantiate(_prefab, _container);
                _createdItems.Add(item);
            }
            
            for (int i = 0; i<_inventory.Length; i++)
            {
                _createdItems[i].SetData(_inventory[i], i);
                _createdItems[i].gameObject.SetActive(true);
            }

            for (int i = _inventory.Length; i < _createdItems.Count; i++)
            {
                _createdItems[i].gameObject.SetActive(false);
            }
        }
    }
}