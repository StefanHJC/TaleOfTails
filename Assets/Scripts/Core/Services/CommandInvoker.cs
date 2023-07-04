using System;
using System.Collections.Generic;
using Core.Logic;
using Core.Services.Input;
using Core.Unit;
using Core.Unit.Ability;
using Core.Unit.Command;
using UnityEngine;
using Zenject;

namespace Core.Services
{
    public class CommandInvoker : ICommandInvoker
    {
        private readonly IInputService _input;
        private readonly IUnitCommandBuilder _unitCommandBuilder;
        private readonly IUnitActionsResolver _unitActions;
        private readonly ITurnResolver _turnResolver;

        public event Action CommandSent;
        public event Action CommandExecuted;

        [Inject]
        public CommandInvoker(IUnitCommandBuilder commandBuilder, IUnitActionsResolver unitActions, ITurnResolver turnResolver)
        {
            _unitCommandBuilder = commandBuilder;
            _unitActions = unitActions;
            _turnResolver = turnResolver;
        }

        //TODO refactor this shit
        public void SendMoveCommand(BaseUnit unit, Cell moveTarget)
        {
            Queue<IUnitCommand> command = _unitCommandBuilder.BuildMove(unit, moveTarget);
            unit.CommandReceiver.CommandExecuted += OnCommandExecuted;

            CommandSent?.Invoke();
            unit.CommandReceiver.ExecuteCommand(command);
        }

        public void SendMeleeAttackCommand(BaseUnit unit, BaseUnit attackTarget, Cell from) // TODO refactor!!!
        {
            Queue<IUnitCommand> command = _unitCommandBuilder.BuildAttack(unit, attackTarget, from, AttackTypeId.Melee);
            unit.CommandReceiver.CommandExecuted += OnCommandExecuted;
            
            CommandSent?.Invoke();
            unit.CommandReceiver.ExecuteCommand(command);
        }

        public void SendRangeAttackCommand(BaseUnit unit, BaseUnit attackTarget) // TODO refactor!!!
        {
            Queue<IUnitCommand> command = _unitCommandBuilder.BuildAttack(unit, attackTarget, unit.CurrentCell, AttackTypeId.Ranged);
            unit.CommandReceiver.CommandExecuted += OnCommandExecuted;

            CommandSent?.Invoke();
            unit.CommandReceiver.ExecuteCommand(command);
        }

        public void SendAbilityExecuteCommand(BaseUnit caster, Cell target, BaseAbility ability)
        {
            Queue<IUnitCommand> command = _unitCommandBuilder.BuildAbility(caster, target, ability);
            caster.CommandReceiver.CommandExecuted += OnCommandExecuted;

            CommandSent?.Invoke();
            caster.CommandReceiver.ExecuteCommand(command);
        }

        public void SendSkipTurnCommand(BaseUnit unit)
        {
            unit.CommandReceiver.CommandExecuted += OnCommandExecuted;

            var command = new SkipTurnCommand();
            var commandQueue = new Queue<IUnitCommand>();
            commandQueue.Enqueue(command);

            CommandSent?.Invoke();
            unit.CommandReceiver.ExecuteCommand(commandQueue);
        }

        private void OnCommandExecuted(ICommandReceiver receiver)
        {
            receiver.CommandExecuted -= OnCommandExecuted;
            CommandExecuted?.Invoke();
        }
    }
}