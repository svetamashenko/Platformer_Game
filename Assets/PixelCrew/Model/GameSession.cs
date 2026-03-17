using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using PixelCrew.Model.Data;

namespace PixelCrew.Model
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _data; private readonly Dictionary<string, PlayerData> _storage = new Dictionary<string, PlayerData>(); public PlayerData Data => _data; private string _currentScene;


        private void Awake()
        {
            LoadHud();

            if (FindObjectOfType<GameSession>() != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            UpdateScene();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void LoadHud()
        {
            SceneManager.LoadScene("Hud", LoadSceneMode.Additive);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateScene();
        }

        private void UpdateScene()
        {
            _currentScene = SceneManager.GetActiveScene().name;

            var keysToRemove = _storage.Keys.Where(k => k != _currentScene).ToList();
            foreach (var key in keysToRemove)
                _storage.Remove(key);

            if (!_storage.ContainsKey(_currentScene))
            {
                _storage[_currentScene] = _data.Clone();
            }
        }

        public void ResetToInitialState()
        {
            if (_storage.TryGetValue(_currentScene, out var initial))
            {
                _data.CopyFrom(initial);
            }
        }

    }
}