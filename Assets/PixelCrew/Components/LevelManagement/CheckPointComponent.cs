using PixelCrew.Components.GoBased;
using PixelCrew.Model;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.PixelCrew.Components.LevelManagement
{
    [RequireComponent(typeof(SpawnComponent))]
    public class CheckPointComponent : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField] private UnityEvent _setChecked;
        [SerializeField] private UnityEvent _setUnchecked;

        public string Id => _id;
        private GameSession _session;
        private SpawnComponent _heroSpawner;

        private void Awake()
        {
            _session = FindObjectOfType<GameSession>();
        }

        private void Start()
        {
            _heroSpawner = GetComponent<SpawnComponent>();
            if (_session != null && _session.IsChecked(_id))
            {
                _setChecked?.Invoke();
            }
            else
            {
                _setUnchecked?.Invoke();
            }
        }

        public void Check()
        {
            if (_session != null)
            {
                _session.SetChecked(_id);
                _setChecked?.Invoke();
            }
        }

        public void SpawnHero()
        {
            if (_heroSpawner == null)
            {
                _heroSpawner = GetComponent<SpawnComponent>();
                if (_heroSpawner == null) return;
            }
            _heroSpawner.Spawn();
        }
    }
}