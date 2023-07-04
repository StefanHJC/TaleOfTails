using System.Collections.Generic;
using Core.Logic;
using Core.Logic.Grid;
using Core.Services.Pathfinder;
using Core.Unit;
using Core.Unit.Ability;
using Core.Unit.Command;
using UnityEngine;
using Zenject;
using Path = Core.Services.Pathfinder.Path;

namespace Core.Services
{
    public class UnitCommandBuilder : IUnitCommandBuilder
    {
        private readonly IPathfinderService _pathfinder;

        [Inject]
        public UnitCommandBuilder(AbstractGrid grid, IPathfinderService pathfinder)
        {
            _pathfinder = pathfinder;
        }
        
        public Queue<IUnitCommand> BuildAttack(BaseUnit commandReceiver, BaseUnit target, Cell from, AttackTypeId attackType)
        {
            Queue<IUnitCommand> attackCommand = BuildMove(commandReceiver, from);
            attackCommand.Enqueue(new RotateCommand(to: target.CurrentCell));
            attackCommand.Enqueue(new AttackCommand(target, attackType)); // temp

            if (attackType == AttackTypeId.Ranged)
            {
                attackCommand.Clear();
                attackCommand.Enqueue(new RotateCommand(to: target.CurrentCell));
                attackCommand.Enqueue(new AttackCommand(target, AttackTypeId.Ranged));
            } // TODO rework

            return attackCommand;
        }

        public Queue<IUnitCommand> BuildMove(BaseUnit commandReceiver, Cell targetCell)
        {
            Queue<IUnitCommand> command = new Queue<IUnitCommand>();
            Quaternion lastRotation = new Quaternion(0, 0, 0, -1);
            Path path = _pathfinder.GetPath(from: commandReceiver.CurrentCell, to: targetCell);

            for (int i = 1; i < path.List.Count; ++i)
            {
                var targetRotation =
                    Quaternion.LookRotation(path.List[i - 1].transform.position - path.List[i].transform.position);

                if (targetRotation != lastRotation)
                {
                    command.Enqueue(new RotateCommand(to: path.List[i]));
                    lastRotation = targetRotation;
                }
                command.Enqueue(new MoveCommand(to: path.List[i]));
                
            }
            return command;
        }

        public Queue<IUnitCommand> BuildAbility(BaseUnit commandReceiver, Cell target, BaseAbility baseAbility)
        {
            Queue<IUnitCommand> abilityCommand = new Queue<IUnitCommand>();
            abilityCommand.Enqueue(new RotateCommand(to: target));
            abilityCommand.Enqueue(new AbilityCommand(target, baseAbility, commandReceiver));

            return abilityCommand;
        }
    }
}