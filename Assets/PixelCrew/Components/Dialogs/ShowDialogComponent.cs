using Assets.PixelCrew.Model.Data;
using Assets.PixelCrew.Model.Definitions;
using Assets.PixelCrew.Model.Definitions.Localization;
using Assets.PixelCrew.Model.UI.Dialogs;
using System;
using UnityEngine;

namespace Assets.PixelCrew.Components.Dialogs
{
    public class ShowDialogComponent : MonoBehaviour
    {
        [SerializeField] private Mode _mode;
        [SerializeField] private DialogData _bound;
        [SerializeField] private DialogDef _external;
        [SerializeField] private Sprite _npcAvatar;

        private DialogBoxController _dialogBox;
        private bool _isShowing;

        private void Awake()
        {
            LocalizationManager.I.OnLocaleChanged += OnLocaleChanged;
        }

        private void OnLocaleChanged()
        {
            if (_isShowing && _dialogBox != null)
            {
                _dialogBox.ShowDialog(Data, GetNpcSprite());
            }
        }

        public void Show()
        {
            if (_dialogBox == null)
                _dialogBox = FindObjectOfType<DialogBoxController>();

            _isShowing = true;
            _dialogBox.ShowDialog(Data, GetNpcSprite());
        }

        private Sprite GetNpcSprite()
        {
            Sprite npcSprite = _npcAvatar;
            if (npcSprite == null)
            {
                var sr = GetComponent<SpriteRenderer>();
                if (sr != null) npcSprite = sr.sprite;
            }
            return npcSprite;
        }

        public DialogData Data
        {
            get
            {
                switch (_mode)
                {
                    case Mode.Bound: return _bound;
                    case Mode.External: return _external.Data;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnDestroy()
        {
            LocalizationManager.I.OnLocaleChanged -= OnLocaleChanged;
        }

        public enum Mode { Bound, External }
    }
}