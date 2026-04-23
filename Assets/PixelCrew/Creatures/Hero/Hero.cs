using PixelCrew.Utils;
using PixelCrew.Components.Health;
using PixelCrew.Components.ColliderBased;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using Assets.PixelCrew.Creatures;
using PixelCrew.Model;
using PixelCrew.Components.GoBased;
using PixelCrew.Model.Definitions;
using Assets.PixelCrew.Model.Definitions;
using PixelCrew.Model.Data;

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

        [SerializeField] private SpawnComponent _throwSpawner;

        private bool _allowDoubleJump;
        private GameSession _session;
        private HealthComponent _health;

        private const float AnimationDuration = 0.16f;

        private const string SwordId = "Sword";
        private int SwordCount => _session.Data.Inventory.Count(SwordId);

        private int CoinCount => _session.Data.Inventory.Count("Coin");
        private string SelectedItemId => _session.QuickInventory.SelectedItem.Id;
        private int ThrowableCount => _session.QuickInventory.SelectedItem.Value;

        private bool CanThrow
        {
            get
            {
                if (SelectedItemId == SwordId)
                    return SwordCount > 1;

                var def = DefsFacade.I.Items.Get(SelectedItemId);
                return def.HasTag(ItemTag.Throwable);
            }
        }

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
            if (id == SwordId)
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

        public void NextItem()
        {
            _session.QuickInventory.SetNextItem();
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

            if (!CanThrow)
            {
                Debug.Log("Throwing is impossible!");
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
            Debug.Log($"Hero has {ThrowableCount} projectiles of current type.");
        }

        public void OnThrowApplying()
        {
            Sounds.Play("Range");
            var throwableId = SelectedItemId;
            var throwableDef = DefsFacade.I.ThrowableItems.Get(throwableId);
            _throwSpawner.SetPrefab(throwableDef.Projectile);
            _throwSpawner.Spawn();
            _session.Data.Inventory.Remove(throwableId, 1);
        }

        public void ThrowMultiple(int numberOfSwords)
        {
            if (ThrowableCount <= 1) return;

            int maxPossible = Mathf.Min(numberOfSwords, SelectedItemId == SwordId ? ThrowableCount - 1 : ThrowableCount);

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

        public void UseSelectedItem()
        {
            var selectedItem = _session.QuickInventory.SelectedItem;
            if (selectedItem == null) return;

            var itemId = selectedItem.Id;

            if (itemId.StartsWith("HealthPotion"))
            {
                var healingAmount = GetHealingFromPotionId(itemId);
                if (healingAmount > 0 && _session.Data.Inventory.Count(itemId) > 0)
                {
                    Sounds.Play("Drink");
                    _health.ModifyHealth(healingAmount);
                    _session.Data.Inventory.Remove(itemId, 1);
                }
            }
            else if (itemId.StartsWith("SpeedPotion"))
            {
                if (_session.Data.Inventory.Count(itemId) > 0)
                {
                    Sounds.Play("Drink");

                    var speedEffect = new GameObject("SpeedEffect");
                    speedEffect.transform.SetParent(transform);
                    var speedComponent = speedEffect.AddComponent<SpeedUpComponent>();
                    speedComponent.Activate(this);

                    _session.Data.Inventory.Remove(itemId, 1);
                }
            }
        }

        private int GetHealingFromPotionId(string potionId)
        {
            if (potionId == "HealthPotion1") return 1;
            if (potionId == "HealthPotion5") return 5;
            return 0;
        }

        public InventoryItemData GetSelectedItem()
        {
            return _session.QuickInventory.SelectedItem;
        }
    }
}