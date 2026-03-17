using Assets.PixelCrew.Model.Data;
using Assets.PixelCrew.Components.UI.Widgets;
using UnityEngine;

namespace Assets.PixelCrew.Components.UI.MainMenu
{
    public class SettingsWindow : AnimatedWindow
    {
        [SerializeField] private AudioSettingsWidget _music;
        [SerializeField] private AudioSettingsWidget _sfx;

        protected override void Start()
        {
            base.Start();

            _music.SetModel(GameSettings.I.Music);
            _sfx.SetModel(GameSettings.I.Sfx);
        }
    }
}