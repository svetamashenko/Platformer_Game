using Assets.PixelCrew.Model;
using Assets.PixelCrew.Utils;
using PixelCrew;
using PixelCrew.Components;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.PixelCrew.Creatures
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
        private int _swordsNumber;
        private int _maxSwordsNumber = 5;

        private const float AnimationDuration = 0.16f;

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();
            var health = GetComponent<HealthComponent>();

            health.SetHealth(_session.Data.Hp);
            UpdateHeroWeapon();
            _swordsNumber = _session.Data.IsArmed ? 1 : 0;
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
            if (_swordsNumber == 5)
            {
                Debug.Log($"Hero already has max number of swords ({_maxSwordsNumber})!");
            }
            else
            {
                _swordsNumber += 1;
                Debug.Log($"Hero has {_swordsNumber} swords.");
            }
        }

        public void Throw()
        {

            if (_swordsNumber == 1)
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
            _swordsNumber -= 1;
            Debug.Log($"Hero has {_swordsNumber} swords.");
        }

        public void OnThrowApplying()
        {
            Particles.Spawn("Throw");
        }

        public void ThrowMultiple(int numberOfSwords)
        {
            if (_swordsNumber <= 1) return;

            int maxPossible = Mathf.Min(numberOfSwords, _swordsNumber - 1);

            if (maxPossible <= 0) return;

            if (_throwCooldown.IsReady)
            {
                StartCoroutine(ThrowSequence(maxPossible));
            }
        }

        private IEnumerator ThrowSequence(int count)
        {
            for (int i = 0; i < count; i++)
            {
                PerformThrow();
                yield return new WaitForSeconds(AnimationDuration);
            }
        }
    }
}
