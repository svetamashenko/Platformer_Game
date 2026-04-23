using Assets.PixelCrew.Utils.Disposables;
using PixelCrew.Model;
using PixelCrew.Model.Data;
using PixelCrew.Model.Definitions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PixelCrew.Model.UI.Hud.QuickInventory
{
    public class InventoryItemWidget : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _selection;
        [SerializeField] private Text _value;

        private readonly CompositeDisposable _trash = new CompositeDisposable();

        private int _index;

        public void OnIndexChanged (int newValue, int _)
        {
            if (_selection == null) return;
            _selection.SetActive(_index == newValue);
        }

        private void Start()
        {
            var session = FindObjectOfType<GameSession>();
            session.QuickInventory.SelectedIndex.SubscribeAndInvoke(OnIndexChanged);
        }

        public void SetData(InventoryItemData item, int index)
        {
            _index = index;

            var def = DefsFacade.I.Items.Get(item.Id);
            _icon.sprite = def.Icon;
            _value.text = !def.HasTag(Definitions.ItemTag.Stackable) ? string.Empty : item.Value.ToString();
        }
    }
}