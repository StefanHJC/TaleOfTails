using System;
using Core.Unit.Command;
using UnityEngine;

namespace Core.Unit.Component
{
    public class UnitMeleeAttack : MonoBehaviour, IAttackCommandHandler
    {
        [SerializeField] private BaseUnitAnimator _animator;
        [SerializeField] private BaseUnit _unit;

        private BaseUnit _target;
        private bool _isAttacking;

        public CommandTypeId HandlerType => CommandTypeId.Attack;
        public AttackTypeId AttackType => AttackTypeId.Melee;

        public event Action Executed;

        public void Execute(IUnitCommand command, Action onExecuted)
        {
            AttackCommand attackCommand = (AttackCommand)command;

            Executed = onExecuted;
            Attack(attackCommand.Target);
        }
        
        public void Attack(BaseUnit target)
        {
            _isAttacking = true;
            _target = target;
            _target.GetComponent<UnitRotate>().Rotate(to: _unit.CurrentCell);
            _animator.PlayMeleeAttack();
            _unit.Sound.PlayOnce(_unit.Config.MeleeAttackStartSFX, transform.position);
        }

        private void OnAttackEnd()
        {
            if (_isAttacking == false) 
                return;

            _target.GetComponent<UnitHealth>().TakeDamage(_unit.Config.Damage, CallHitback);
            _unit.Sound.PlayOnce(_unit.Config.MeleeAttackHitSFX, transform.position);
            _unit.ActionPoints = 0;

            _isAttacking = false;
        }

        private void CallHitback()
        {
            _target.GetComponent<UnitMeleeHitBack>().TryHitBack(_unit, ref Executed);

            _target = null;
        }
    }
}