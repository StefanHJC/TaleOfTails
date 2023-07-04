using System;
using Core.Unit.Command;
using UnityEngine;

namespace Core.Unit.Ability
{
    public class BaseUnitAbilityExecuter : MonoBehaviour, IUnitCommandHandler
    {
        [SerializeField] private BaseUnit _caster;

        private BaseAbility _currentAbility;

        public CommandTypeId HandlerType => CommandTypeId.UseAbility;
        
        public event Action Executed;

        public void Execute(IUnitCommand command, Action onExecuted)
        {
            AbilityCommand castedCommand = (AbilityCommand)command;
            _caster.ActionPoints = 0;
            _currentAbility = castedCommand.Ability;
            Executed = onExecuted;

            _currentAbility.Perform(castedCommand.Target, Executed);
        }

        private void OnAnimatorPerform() => _currentAbility.OnAnimatorPerform();
    }
}