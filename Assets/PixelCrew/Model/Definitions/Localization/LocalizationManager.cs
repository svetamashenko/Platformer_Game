using Assets.PixelCrew.Model.Data.Properties;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PixelCrew.Model.Definitions.Localization
{
    public class LocalizationManager
    {
        public readonly static LocalizationManager I;

        private StringPersistentProperty _localeKey = new StringPersistentProperty("en", "localization/current");
        private Dictionary<string, string> _localization;
        private Dictionary<string, Sprite> _localizationImages;

        public event Action OnLocaleChanged;

        public string LocaleKey => _localeKey.Value;

        static LocalizationManager()
        {
            I = new LocalizationManager();
        }

        public LocalizationManager()
        {
            _localizationImages = new Dictionary<string, Sprite>();
            LoadLocale(_localeKey.Value);
        }

        public void LoadLocale(string localeToLoad)
        {
            var def = Resources.Load<LocaleDef>($"Locales/{localeToLoad}");
            _localization = def.GetData();
            _localeKey.Value = localeToLoad;

            LoadImages(localeToLoad);

            OnLocaleChanged?.Invoke();
        }

        private void LoadImages(string localeKey)
        {
            _localizationImages.Clear();

            var sprites = Resources.LoadAll<Sprite>($"Locales/Images/{localeKey}");
            foreach (var sprite in sprites)
            {
                _localizationImages[sprite.name] = sprite;
            }
        }

        public string Localize(string key)
        {
            return _localization.TryGetValue(key, out var value) ? value : $"%%%{key}%%%";
        }

        public Sprite LocalizeImage(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName)) return null;

            if (_localizationImages.TryGetValue(spriteName, out var sprite))
                return sprite;

            return null;
        }

        public void SetLocale(string localeKey)
        {
            LoadLocale(localeKey);
        }
    }
}