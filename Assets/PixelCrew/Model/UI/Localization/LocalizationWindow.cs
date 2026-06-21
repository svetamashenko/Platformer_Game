using Assets.PixelCrew.Components.UI;
using Assets.PixelCrew.Model.Definitions.Localization;
using Assets.PixelCrew.Model.UI.Widgets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PixelCrew.Model.UI.Localization
{
    public class LocalizationWindow : AnimatedWindow
    {
        [SerializeField] private Transform _container;
        [SerializeField] private LocaleItemWidget _prefab;

        private DataGroup<LocaleInfo, LocaleItemWidget> _dataGroup;

        private string[] _supportedLocales = new[] { "en", "ru", "fr" };

        protected override void Start()
        {
            base.Start();
            _dataGroup = new DataGroup<LocaleInfo, LocaleItemWidget>(_prefab, _container);
            _dataGroup.SetData(ComposeData());
        }

        public void UpdateDataGroup()
        {
            _dataGroup.SetData(ComposeData());
        }

        private List<LocaleInfo> ComposeData()
        {
            var data = new List<LocaleInfo>();
            foreach (var locale in _supportedLocales)
            {
                data.Add(new LocaleInfo { LocaleId = locale });
            }
            return data;
        }

        public void OnSelected(string selectedLocale)
        {
            LocalizationManager.I.SetLocale(selectedLocale);
        }
    }
}