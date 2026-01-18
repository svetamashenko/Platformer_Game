using Assets.PixelCrew.Utils;
using PixelCrew;
using PixelCrew.Components;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.PixelCrew.Creatures
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

        private Animator _animator;
        private bool _hasFiredOnce;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _hasFiredOnce = false;
        }

        private void Update()
        {
            if (_vision.IsTouchingLayer)
            {
                // Сброс флага при входе в зону видимости
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
                // Сброс флага, когда игрок выходит из зоны
                _hasFiredOnce = false;
            }
        }

        private void RangeAttack()
        {
            if (_hasRangeAttack)
            {
                _animator.SetTrigger(RangeAttackKey);
            }
        }

        private void MeleeAttack()
        {
            if (_hasMeleeAttack)
            {
                _animator.SetTrigger(MeleeAttackKey);
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
