using System.Collections.Generic;
using Core.Logic;
using Core.Unit;
using Core.Unit.Ability;
using Core.Unit.Command;

namespace Core.Services
{
    public interface IUnitCommandBuilder
    {
        public Queue<IUnitCommand> BuildAttack(BaseUnit commandReceiver, BaseUnit target, Cell from, AttackTypeId attackType);
        public Queue<IUnitCommand> BuildMove(BaseUnit commandReceiver, Cell targetCell);
        public Queue<IUnitCommand> BuildAbility(BaseUnit commandReceiver, Cell target, BaseAbility baseAbility);
    }
}