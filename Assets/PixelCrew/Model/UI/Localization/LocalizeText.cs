using Assets.PixelCrew.Model.Definitions.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PixelCrew.Model.UI.Localization
{
    public class LocalizeText : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField] private bool _capitalize;
        [SerializeField] private bool _capitalizeFirstLetter;

        private Text _text;

        private void Awake()
        {
            _text = GetComponent<Text>();

            LocalizationManager.I.OnLocaleChanged += OnLocaleChanged;
            Localize();
        }

        private void OnLocaleChanged()
        {
            Localize();
        }

        private void Localize()
        {
            var localized = LocalizationManager.I.Localize(_key);

            if (_capitalize)
            {
                localized = localized.ToUpper();
            }
            else if (_capitalizeFirstLetter)
            {
                localized = CapitalizeFirstLetter(localized);
            }

            _text.text = localized;
        }

        private string CapitalizeFirstLetter(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (text.Length == 1)
                return text.ToUpper();

            return char.ToUpper(text[0]) + text.Substring(1);
        }

        private void OnDestroy()
        {
            LocalizationManager.I.OnLocaleChanged -= OnLocaleChanged;
        }
    }
}