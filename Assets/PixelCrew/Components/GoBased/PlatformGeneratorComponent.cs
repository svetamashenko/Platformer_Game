using UnityEngine;

namespace PixelCrew.Components.GoBased
{
    public class PlatformGeneratorComponent : MonoBehaviour
    {
        [SerializeField] private GameObject _platformPrefab;
        [SerializeField] private float _gridSize = 1f;
        [SerializeField] private float _collisionCheckRadius = 0.4f;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private Collider2D _boundsCollider;
        [SerializeField] private float _cooldown = 5f;

        private float _verticalOffset = -1f;
        private float _nextAllowedGenerationTime;

        public bool CanGenerate => Time.time >= _nextAllowedGenerationTime;

        public void GeneratePlatform(Transform heroTransform)
        {
            if (!CanGenerate)
                return;

            Vector3 heroPosition = heroTransform.position;

            float gridX = Mathf.Round(heroPosition.x / _gridSize) * _gridSize;
            float gridY = Mathf.Round(heroPosition.y / _gridSize) * _gridSize;

            Vector3 spawnPosition = new Vector3(
                gridX,
                gridY + _verticalOffset,
                heroPosition.z);

            if (!_boundsCollider.OverlapPoint(spawnPosition))
                return;

            Collider2D[] overlaps = Physics2D.OverlapCircleAll(spawnPosition, _collisionCheckRadius, _obstacleMask);
            foreach (Collider2D collider in overlaps)
            {
                if (collider.transform == heroTransform) continue;
                return;
            }

            var platform = Instantiate(_platformPrefab, spawnPosition, Quaternion.identity);
            AudioSource Sounds = platform.GetComponent<AudioSource>();
            Sounds.Play();
            _nextAllowedGenerationTime = Time.time + _cooldown;
        }
    }
}
