using Assets.PixelCrew.Creatures;
using UnityEngine;

namespace PixelCrew.Components.GoBased
{
    public class SlowDownComponent : MonoBehaviour
    {
        [SerializeField] private int slowFactor = 1;
        private Creature _creature;

        public void Activate(Creature creature)
        {
            _creature = creature;
            _creature.SetSpeed(slowFactor);
        }

        public void Deactivate()
        {
            RestoreOriginalSpeed();
            _creature = null;
        }

        private void RestoreOriginalSpeed()
        {
            if (_creature != null)
            {
                _creature.ResetSpeed();
            }
        }
    }
}