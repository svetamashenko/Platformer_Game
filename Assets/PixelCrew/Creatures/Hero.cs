using Assets.PixelCrew.Model;
using PixelCrew;
using PixelCrew.Components;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.PixelCrew.Creatures
{
    public class Hero : Creature
    {
        [SerializeField] private float _interactionRadius;
        [SerializeField] private LayerMask _layer;
        [SerializeField] private AnimatorController _armed;
        [SerializeField] private AnimatorController _disarmed;
        [SerializeField] private ParticleSystem _hitParticles;
        [SerializeField] private CheckCircleOverlap _interactionCheck;

        private bool _allowDoubleJump;
        private GameSession _session;

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            var health = GetComponent<HealthComponent>();

            health.SetHealth(_session.Data.Hp);
            UpdateHeroWeapon();
        }
        private void UpdateHeroWeapon()
        {
            Animator.runtimeAnimatorController = _session.Data.IsArmed ? _armed : _disarmed;
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
                Particles.Spawn("Jump");
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
            if (_session.Data.Coins > 0)
            {
                SpawnCoins();
            }
        }

        private void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(_session.Data.Coins, 5);
            _session.Data.Coins -= numCoinsToDispose;
            ShowBalance();

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);

            _hitParticles.gameObject.SetActive(true);
            _hitParticles.Play();
        }

        public void Interact()
        {
            _interactionCheck.Check();
        }

        public void AddToBalance(int balance)
        {
            _session.Data.Coins += balance;
            ShowBalance();
        }

        private void ShowBalance()
        {
            Debug.Log($"Player has {_session.Data.Coins} currency.");
        }

        public void ResetBalance()
        {
            _session.Data.Coins = 0;
        }

        public override void Attack()
        {
            if (!_session.Data.IsArmed)
            {
                return;
            }

            base.Attack();
        }

        public void OnHealthChanged(int currentHealth)
        {
            _session.Data.Hp = currentHealth;
        }

        public void ArmHero()
        {
            _session.Data.IsArmed = true;
            Animator.runtimeAnimatorController = _armed;
        }
    }
}
