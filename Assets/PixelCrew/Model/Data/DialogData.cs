using System;
using UnityEngine;

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

        public string GetSentence(int index) => _sentences[index].Text;
        public bool IsPlayerSpeaking(int index) => _sentences[index].IsPlayer;
    }
}