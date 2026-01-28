using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components.Animations
{
    [System.Serializable]
    public struct AnimationState
    {
        public string Name;
        public bool Loop;
        public Sprite[] Sprites;
        public bool AllowNext;
        public string NextStateName;
        public UnityEvent OnComplete;
    }
}
