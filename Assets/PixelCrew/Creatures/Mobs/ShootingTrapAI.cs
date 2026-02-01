using PixelCrew.Utils;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Components.GoBased;
using UnityEngine;

namespace Assets.PixelCrew.Creatures.Mobs
{
    public class ShootingTrapAI : MonoBehaviour
    {
        [SerializeField] private LayerCheck _vision;

        [Header("Melee")]
        [SerializeField] private bool _hasMeleeAttack = true;
        [SerializeField] private Cooldown _meleeCooldown;
        [SerializeField] private CheckCircleOverlap _meleeAttack;
        [SerializeField] private LayerCheck _meleeCanAttack;

        [Header("Range")]
        [SerializeField] private bool _hasRangeAttack = true;
        [SerializeField] private Cooldown _rangeCooldown;
        [SerializeField] private SpawnComponent _rangeAttack;

        [Header("Delay")]
        [Range(0, 100)]
        [SerializeField]
        private int _initialDelayPercent = 0;

        private static readonly int MeleeAttackKey = Animator.StringToHash("melee");
        private static readonly int RangeAttackKey = Animator.StringToHash("range");

        private PlaySoundsComponent _sounds;
        private Animator _animator;
        private bool _hasFiredOnce;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _sounds = GetComponent<PlaySoundsComponent>();
            _hasFiredOnce = false;
        }

        private void Update()
        {
            if (_vision.IsTouchingLayer)
            {
                if (!_hasFiredOnce)
                {
                    _rangeCooldown.ResetWithDelayPercent(_initialDelayPercent);
                    _hasFiredOnce = true;
                }

                if (_hasMeleeAttack)
                {
                    if (_meleeCanAttack.IsTouchingLayer)
                    {
                        if (_meleeCooldown.IsReady)
                        {
                            MeleeAttack();
                            _meleeCooldown.Reset();
                            return;
                        }
                    }
                }

                if (_hasRangeAttack)
                {
                    if (_rangeCooldown.IsReady)
                    {
                        RangeAttack();
                        _rangeCooldown.Reset();
                    }
                }
            }
            else
            {
                _hasFiredOnce = false;
            }
        }

        private void RangeAttack()
        {
            if (_hasRangeAttack)
            {
                _animator.SetTrigger(RangeAttackKey);
                _sounds.Play("Range");
            }
        }

        private void MeleeAttack()
        {
            if (_hasMeleeAttack)
            {
                _animator.SetTrigger(MeleeAttackKey);
                _sounds.Play("Melee");
            }
        }

        private void OnRangeAttack()
        {
            if (_hasRangeAttack)
            {
                _rangeAttack.Spawn();
            }
        }

        private void OnMeleeAttack()
        {
            if (_hasMeleeAttack)
            {
                _meleeAttack.Check();
            }
        }
    }
}
