using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimation : MonoBehaviour
    {
        [SerializeField] private List<AnimationState> _states = new List<AnimationState>();

        [SerializeField] private int _frameRate = 10;
        [SerializeField] private UnityEvent _onComplete;

        private SpriteRenderer _renderer;
        private AnimationState _currentState;
        private float _secondsPerFrame;
        private int _currentSpriteIndex;
        private float _nextFrameTime;
        private bool _isPlaying = true;


        private void OnBecomeVisible()
        {
            enabled = _isPlaying;
        }

        private void OnBecomeInvisible()
        {
            enabled = false;
        }

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();

            if (_states.Count > 0)
            {
                _currentState = _states[0];
                _secondsPerFrame = 1f / _frameRate;
                _nextFrameTime = Time.time + _secondsPerFrame;
            }
        }

        private void Update()
        {
            if (!_isPlaying || _nextFrameTime > Time.time) return;

            if (_currentSpriteIndex >= _currentState.Sprites.Length)
            {
                if (_currentState.Loop)
                {
                    _currentSpriteIndex = 0;
                }
                else
                {
                    _isPlaying = false;
                    if (_currentState.AllowNext)
                    {
                        _currentState = FindStateByName(_currentState.NextStateName);
                        if (!string.IsNullOrEmpty(_currentState.Name))
                        {
                            _currentSpriteIndex = 0;
                            _isPlaying = true;
                        }
                    }
                    _onComplete?.Invoke();
                    return;
                }
            }

            _renderer.sprite = _currentState.Sprites[_currentSpriteIndex];
            _nextFrameTime += _secondsPerFrame;
            _currentSpriteIndex++;
        }

        public void SetClip(string name)
        {
            _currentState = FindStateByName(name);
            if (!string.IsNullOrEmpty(_currentState.Name))
            {
                _currentSpriteIndex = 0;
                _secondsPerFrame = 1f / _frameRate;
                _nextFrameTime = Time.time + _secondsPerFrame;
                _isPlaying = true;
            }

            enabled = _isPlaying = false;
        }

        private AnimationState FindStateByName(string name)
        {
            foreach (var state in _states)
            {
                if (state.Name == name)
                {
                    return state;
                }
            }
            return new AnimationState();
        }
    }
}
