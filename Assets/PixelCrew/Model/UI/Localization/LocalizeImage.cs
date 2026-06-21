using Assets.PixelCrew.Model.Definitions.Localization;
using UnityEngine;

namespace Assets.PixelCrew.Model.UI.Localization
{
    public class LocalizeImage : MonoBehaviour
    {
        [SerializeField] private string _spriteName;

        private SpriteRenderer _spriteRenderer;
        private Sprite _defaultSprite;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null) return;

            _defaultSprite = _spriteRenderer.sprite;

            LocalizationManager.I.OnLocaleChanged += OnLocaleChanged;
            Localize();
        }

        private void OnLocaleChanged()
        {
            Localize();
        }

        private void Localize()
        {
            if (_spriteRenderer == null || string.IsNullOrEmpty(_spriteName)) return;

            var sprite = LocalizationManager.I.LocalizeImage(_spriteName);

            if (sprite != null)
                _spriteRenderer.sprite = sprite;
            else
                _spriteRenderer.sprite = _defaultSprite;
        }

        private void OnDestroy()
        {
            LocalizationManager.I.OnLocaleChanged -= OnLocaleChanged;
        }
    }
}