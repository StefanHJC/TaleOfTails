using System;
using Core.Database;
using Core.Logic;
using Core.Services;
using Core.Services.Input;
using Core.Unit.Ability;
using Utils.TypeBasedFSM;
using Zenject;

namespace Core.Infrastructure.GameFSM.GameLoop.LocalInput
{
    public class AbilityInputState : IPayloadedState<BaseAbility>
    {
        private readonly AbilityInputStateMachine _stateMachine;
        private readonly LocalInputStateMachine _localInputStateMachine;
        private readonly Cursor _cursor;
        private readonly IInputService _input;
        private readonly ICommandInvoker _commandService;

        [Inject]
        public AbilityInputState(LocalInputStateMachine localInputStateMachine, IGameFactory gameFactory, IInputService input, Cursor cursor, ICommandInvoker commands)
        {
            _stateMachine = new AbilityInputStateMachine(gameFactory);
            _localInputStateMachine = localInputStateMachine;
            _input = input;
            _cursor = cursor;
            _commandService = commands;
        }

        public void Enter(BaseAbility ability)
        {
            _cursor.State = CursorState.UseAbility;
            OnAbilitySelected(ability);
            _input.CommandButtonPressed += PerformAbility;
        }

        public void Exit()
        {
            _input.CommandButtonPressed -= PerformAbility;
            _stateMachine.Enter<NullAbilityState>();
        }

        private void OnAbilitySelected(BaseAbility selected)
        {
            switch (selected.TypeId)
            {
                case AbilityTypeId.AreaOfEffect:
                    _stateMachine.Enter<AreaOfEffectState, BaseAbility>(selected);
                    break;

                case AbilityTypeId.Target:
                    _stateMachine.Enter<TargetState, BaseAbility>(selected);
                    break;

                default:
                    throw new InvalidOperationException($"Unregistered Ability type {selected.Id}");
            }
        }

        private void PerformAbility()
        {
            switch (_stateMachine.CurrentState)
            {
                case AreaOfEffectState state:
                {
                    if (state.ValidateCommand(_input.FocusedCell))
                    {
                        _commandService.SendAbilityExecuteCommand(state.Parameter.Caster, _input.FocusedCell, state.Parameter);
                        break;
                    }
                    return;
                }
                case TargetState state:
                {
                    if (state.ValidateCommand(_input.FocusedCell))
                    {
                        _commandService.SendAbilityExecuteCommand(state.Parameter.Caster, _input.FocusedCell, state.Parameter);
                        break;
                    }
                    return;
                }
            }
            _localInputStateMachine.Enter<NullCommandState>();
            _stateMachine.Enter<NullAbilityState>();
        }

        private void OnAbilityDeselected()
        {

        }
    }
}