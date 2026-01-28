using UnityEngine;

namespace PixelCrew.Components.GoBased
{
    public class SpawnSwordParticles : MonoBehaviour
    {
        [SerializeField] private SpawnComponent _attackParticles;

        public void SpawnAttackParticles()
        {
            _attackParticles.Spawn();
        }
    }
}
