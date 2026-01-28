using PixelCrew.Components.ColliderBased;
using PixelCrew.Creatures.Hero;
using UnityEngine;

namespace PixelCrew.Components.GoBased
{
    public class BarrelCoinsGenerator : MonoBehaviour
    {
        [SerializeField] private int _maxCoinsNumber;
        [SerializeField] private float _goldProbability;
        [SerializeField] private float _silverProbability;
        [SerializeField] private SpawnComponent _goldSpawn;
        [SerializeField] private SpawnComponent _silverSpawn;
        [SerializeField] private SpawnComponent _destroiedParts;
        [SerializeField] private Hero _hero;

        public void SpawnCoins()
        {
            int numberOfGold = Mathf.RoundToInt(_goldProbability * _maxCoinsNumber);
            int numberOfSilver = _maxCoinsNumber - numberOfGold;

            for (int i = 0; i < numberOfGold; i++)
            {
                GameObject coin = _goldSpawn.Spawn();
                SetupEnterTrigger(coin, 10);
            }

            for (int i = 0; i < numberOfSilver; i++)
            {
                GameObject coin = _silverSpawn.Spawn();
                SetupEnterTrigger(coin, 1);
            }

            _destroiedParts.Spawn();
        }

        private void SetupEnterTrigger(GameObject coin, int value)
        {
            EnterTriggerComponent trigger = coin.GetComponent<EnterTriggerComponent>();

            if (trigger == null)
            {
                return;
            }

            trigger._action = new EnterEvent();

            trigger._action.AddListener((GameObject collider) =>
            {
                _hero.AddToInventory("Coin", value);
                Destroy(coin);
            });
        }
    }
}
