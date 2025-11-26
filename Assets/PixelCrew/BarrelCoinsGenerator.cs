using PixelCrew.Components;
using UnityEngine;

namespace PixelCrew
{
    public class BarrelCoinsGenerator : MonoBehaviour
    {
        [SerializeField] private int _maxCoinsNumber;
        [SerializeField] private float _goldProbability;
        [SerializeField] private float _silverProbability;
        [SerializeField] private SpawnComponent _goldSpawn;
        [SerializeField] private SpawnComponent _silverSpawn;
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
                _hero.AddToBalance(value);
                Destroy(coin);
            });
        }
    }
}
