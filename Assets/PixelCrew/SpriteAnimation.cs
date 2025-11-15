using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace PixelCrew
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimation : MonoBehaviour
    {
        [SerializeField] private bool AddAppearing;
        [SerializeField] public bool AddDisappearing;
        [SerializeField] private List<AnimationState> _states = new List<AnimationState>();
        [SerializeField] private int _frameRate = 10;
        [SerializeField] private UnityEvent _onComplete;
        [SerializeField] private float _fadeDuration = 0.5f;

        private SpriteRenderer _renderer;
        private AnimationState _currentState;
        private float _secondsPerFrame;
        private int _currentSpriteIndex;
        private float _nextFrameTime;
        private bool _isPlaying = true;
        private Color _startColor;

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
            _startColor = _renderer.color;

            if (AddAppearing)
            {
                _renderer.color = new Color(
                    _startColor.r,
                    _startColor.g,
                    _startColor.b,
                    0f);

                StartCoroutine(Appearing());
            }

            if (_states.Count > 0)
            {
                _currentState = _states[0];
                _secondsPerFrame = 1f / _frameRate;
                _nextFrameTime = Time.time + _secondsPerFrame;
            }
        }

        public void StartDisappearing()
        {
            if (!AddDisappearing) return;

            StartCoroutine(Disappearing());
        }

        private void Update()
        {
            if (_states.Count == 0) return;

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
            enabled = _isPlaying = true;
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

        private IEnumerator Appearing()
        {
            float elapsedTime = 0f;
            Color currentColor = _renderer.color;

            while (elapsedTime < _fadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsedTime / _fadeDuration);
                currentColor.a = alpha;
                _renderer.color = currentColor;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentColor.a = 1f;
            _renderer.color = currentColor;
        }

        private IEnumerator Disappearing()
        {
            float elapsedTime = 0f;
            Color currentColor = _renderer.color;

            while (elapsedTime < _fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / _fadeDuration);
                currentColor.a = alpha;
                _renderer.color = currentColor;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
