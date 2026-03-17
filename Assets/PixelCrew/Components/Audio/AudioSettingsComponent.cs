using Assets.PixelCrew.Model.Data;
using Assets.PixelCrew.Model.Data.Properties;
using System;
using UnityEngine;

namespace Assets.PixelCrew.Components.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSettingsComponent : MonoBehaviour
    {
        [SerializeField] private SoundSettings _mode;
        private AudioSource _source;
        private FloatPersistentProperty _model;

        private void Start()
        {
            _source = GetComponent<AudioSource>();

            _model = FindProperty();
            _model.OnChanged += OnSoundSettingsChanged;
            OnSoundSettingsChanged(_model.Value, _model.Value);

        }

        private void OnSoundSettingsChanged(float newValue, float oldValue)
        {
            _source.volume = newValue;
        }

        private FloatPersistentProperty FindProperty()
        {
            switch (_mode)
            {
                case SoundSettings.Music:
                    return GameSettings.I.Music;
                case SoundSettings.Sfx:
                    return GameSettings.I.Sfx;
            }
            throw new ArgumentException("Underfined mode");
        }

        private void OnDestroy()
        {
            _model.OnChanged -= OnSoundSettingsChanged;
        }
    }
}