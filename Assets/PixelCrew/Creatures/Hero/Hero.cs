using PixelCrew.Utils;
using PixelCrew.Components.Health;
using PixelCrew.Components.ColliderBased;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using Assets.PixelCrew.Creatures;
using PixelCrew.Model;

namespace PixelCrew.Creatures.Hero
{
    public class Hero : Creature
    {
        [SerializeField] private float _interactionRadius;
        [SerializeField] private LayerMask _layer;

        [SerializeField] private Cooldown _throwCooldown;
        [SerializeField] private AnimatorController _armed;
        [SerializeField] private AnimatorController _disarmed;
        [SerializeField] private ParticleSystem _coinsParticles;
        [SerializeField] private CheckCircleOverlap _interactionCheck;

        private bool _allowDoubleJump;
        private GameSession _session;
        private HealthComponent _health;

        private const float AnimationDuration = 0.16f;

        private int SwordCount => _session.Data.Inventory.Count("Sword");
        private int CoinCount => _session.Data.Inventory.Count("Coin");
        private int Potions => _session.Data.Inventory.Count("HealthPotion");

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            _health = GetComponent<HealthComponent>();
            _session.Data.Inventory.OnChanged += OnInventoryChanged;

            _health.SetHealth(_session.Data.Hp.Value);
            UpdateHeroWeapon();
        }

        private void OnDestroy()
        {
            _session.Data.Inventory.OnChanged -= OnInventoryChanged;
        }

        public void OnInventoryChanged(string id, int value)
        {
            if (value > 0)
            {
                Debug.Log($"Added {value} of {id}s.");
            }
            else
            {
                Debug.Log($"Removed {-value} of {id}s.");
            }
            if (id == "Sword")
            {
                UpdateHeroWeapon();
            }
        }

        private void UpdateHeroWeapon()
        {
            Animator.runtimeAnimatorController = SwordCount > 0 ? _armed : _disarmed;
        }

        protected override float CalculateVelocity()
        {
            if (IsGrounded)
            {
                _allowDoubleJump = true;
            }

            return base.CalculateVelocity();
        }

        protected override float CalculateJumpVelocity(float yVelocity)
        {
            if (IsGrounded)
            {
                return base.CalculateJumpVelocity(yVelocity);
            }

            else if (_allowDoubleJump && Direction.y > 0)
            {
                _allowDoubleJump = false;
                DoJumpVfx();
                return JumpSpeed;
            }

            return yVelocity;
        }

        public static void SaySomething()
        {
            Debug.Log("Something...");
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            if (CoinCount > 0)
            {
                SpawnCoins();
            }
        }

        private void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(CoinCount, 5);
            _session.Data.Inventory.Remove("Coin", numCoinsToDispose);
            ShowBalance();

            var burst = _coinsParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _coinsParticles.emission.SetBurst(0, burst);

            _coinsParticles.gameObject.SetActive(true);
            _coinsParticles.Play();
        }

        public void Interact()
        {
            _interactionCheck.Check();
        }

        private void ShowBalance()
        {
            Debug.Log($"Player has {CoinCount} currency.");
        }

        public void ResetBalance()
        {
            _session.Data.Inventory.Reset("Coin");
        }

        public override void Attack()
        {
            if (SwordCount == 0)
            {
                return;
            }

            base.Attack();
        }

        public void OnHealthChanged(int currentHealth)
        {
            _session.Data.Hp.Value = currentHealth;
        }

        public void Throw()
        {

            if (SwordCount <= 0)
            {
                Debug.Log("Hero hasn't got swords!");
            }
            else if (SwordCount == 1)
            {
                Debug.Log("Hero can't throw the last one sword!");
            }
            else if (_throwCooldown.IsReady)
            {
                PerformThrow();
            }
        }

        public void PerformThrow()
        {
            Animator.SetTrigger(ThrowKey);
            _throwCooldown.Reset();
            _session.Data.Inventory.Remove("Sword", 1);
            Debug.Log($"Hero has {SwordCount} swords.");
        }

        public void OnThrowApplying()
        {
            Sounds.Play("Range");
            Particles.Spawn("Throw");
        }

        public void ThrowMultiple(int numberOfSwords)
        {
            if (SwordCount <= 1) return;

            int maxPossible = Mathf.Min(numberOfSwords, SwordCount - 1);

            if (maxPossible <= 0) return;

            if (_throwCooldown.IsReady)
            {
                StartCoroutine(ThrowSequence(maxPossible));
            }
        }

        public void AddToInventory(string id, int value)
        {
            _session.Data.Inventory.Add(id, value);
        }

        private IEnumerator ThrowSequence(int count)
        {
            for (int i = 0; i < count; i++)
            {
                PerformThrow();
                yield return new WaitForSeconds(AnimationDuration);
            }
        }

        public void ApplyHealing(int healing)
        {
            if (Potions > 0)
            {
                Sounds.Play("Drink");
                _health.ModifyHealth(healing);
                _session.Data.Inventory.Remove("HealthPotion", 1);
            }
        }
    }
}
