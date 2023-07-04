using System;
using System.Collections.Generic;
using System.Linq;
using Core.UI.HUD;
using Core.Unit.Ability;

namespace Core.Unit.Command
{
    public class BaseUnitCommandReceiver : ICommandReceiver
    {
        private readonly BaseUnit _unit;
        private readonly List<IUnitCommandHandler> _handlers = new List<IUnitCommandHandler>();

        private Queue<IUnitCommand> _commands = new Queue<IUnitCommand>();

        public IReadOnlyList<IUnitCommand> CommandQueue => _commands.ToList();
        public IReadOnlyList<IUnitCommandHandler> CommandHandlers => _handlers;

        public event Action<ICommandReceiver> CommandExecuted;

        public BaseUnitCommandReceiver(BaseUnit unit)
        {
            _unit = unit;

            _handlers = _unit.GetComponents<IUnitCommandHandler>().ToList();
        }

        public void ExecuteCommand(Queue<IUnitCommand> command)
        {
            _commands = command;

            if (_commands.TryDequeue(out IUnitCommand currentAction))
            {
                switch (currentAction.CommandType)
                {
                    case CommandTypeId.Move:
                        ExecuteMoveCommand(currentAction);
                        break;

                    case CommandTypeId.Rotate:
                        ExecuteRotateCommand(currentAction);
                        break;

                    case CommandTypeId.Attack:
                        ExecuteAttackCommand(currentAction);
                        break;

                    case CommandTypeId.UseAbility:
                        ExecuteAbility(currentAction);
                        break;

                    case CommandTypeId.SkipTurn:
                        ExecuteSkipTurn(currentAction);
                        break;
                }
            }
            else if(_commands.Count == 0)
            {
                CommandExecuted?.Invoke(this);
            }
        }

        private void ExecuteAttackCommand(IUnitCommand currentCommand)
        {
            IEnumerable<IAttackCommandHandler> attackHandlers = _handlers
                .Where(handler => handler.HandlerType == CommandTypeId.Attack)
                .Cast<IAttackCommandHandler>();
            
            AttackCommand attackCommand = (AttackCommand)currentCommand;

            IAttackCommandHandler handlerByAttackType = attackHandlers.First(commandHandler => commandHandler.AttackType == attackCommand.AttackType);
            handlerByAttackType.Execute(attackCommand, OnExecuted);
        }

        private void ExecuteAbility(IUnitCommand currentCommand) =>
            _handlers.First(handler => handler.HandlerType == CommandTypeId.UseAbility).Execute(currentCommand, OnExecuted);

        private void ExecuteRotateCommand(IUnitCommand currentCommand) => 
            _handlers.First(handler => handler.HandlerType == CommandTypeId.Rotate).Execute(currentCommand, OnExecuted);

        private void ExecuteMoveCommand(IUnitCommand currentCommand) => 
            _handlers.First(handler => handler.HandlerType == CommandTypeId.Move).Execute(currentCommand, OnExecuted);

        private void ExecuteSkipTurn(IUnitCommand command)
        {
            _unit.ActionPoints = 0;
            _unit.HealthRenderer.RenderStatusMessage("Пропуск хода", HealthRendererSettings.SkipTurn);
            OnExecuted();
        }

        private void OnExecuted() => ExecuteCommand(_commands);
    }
}