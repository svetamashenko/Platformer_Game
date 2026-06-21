using System;
using UnityEngine;
using Assets.PixelCrew.Model.Definitions.Localization;

namespace Assets.PixelCrew.Model.Data
{
    [Serializable]
    public struct SentenceData
    {
        public string Text;
        public bool IsPlayer;
    }

    [Serializable]
    public class DialogData
    {
        [SerializeField] private SentenceData[] _sentences;

        public int Count => _sentences?.Length ?? 0;

        public string GetSentence(int index)
        {
            if (_sentences == null || index >= _sentences.Length)
                return string.Empty;

            var text = _sentences[index].Text;

            // Если текст начинается с "#" - используем как ключ локализации
            if (!string.IsNullOrEmpty(text) && text.StartsWith("#"))
            {
                return LocalizationManager.I.Localize(text.Substring(1));
            }

            return text;
        }

        public bool IsPlayerSpeaking(int index)
        {
            if (_sentences == null || index >= _sentences.Length)
                return false;

            return _sentences[index].IsPlayer;
        }
    }
}