using System;
using UnityEngine;

namespace Core.Unit.Component
{
    public class UnitMeleeHitBack : MonoBehaviour
    {
        [SerializeField] private BaseUnitAnimator _animator;
        [SerializeField] private BaseUnit _unit;

        private BaseUnit _target;
        private bool _isHitbackAttack;

        private event Action Executed;

        public void TryHitBack(BaseUnit target, ref Action onExecuted)
        {
            if (CanHitback())
            {
                onExecuted?.Invoke();
                return;
            }

            _isHitbackAttack = true;
            Attack(target);
            Executed = onExecuted;
        }

        private bool CanHitback() => _unit.GetComponent<UnitHealth>().Health <= 0;

        private void Attack(BaseUnit target)
        {
            _target = target;
            _animator.PlayMeleeAttack();
            _unit.Sound.PlayOnce(_unit.Config.MeleeAttackStartSFX, transform.position);

            if (_unit.IsRanged)
                _unit.GetComponent<UnitRangeAttack>().Target = _target;
        }

        private void OnAttackEnd()
        {
            if (_isHitbackAttack == false)
                return;

            _target.GetComponent<UnitHealth>().TakeDamage(_unit.Config.Damage);
            _unit.Sound.PlayOnce(_unit.Config.MeleeAttackHitSFX, transform.position);
            _target = null;
            _isHitbackAttack = false;
            Executed?.Invoke();
        }
    }
}