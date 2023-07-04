using System;
using Core.Logic;
using Core.Unit;
using Core.Unit.Ability;

namespace Core.Services
{
    public interface ICommandInvoker
    {
        public event Action CommandSent;
        public event Action CommandExecuted;

        public void SendMoveCommand(BaseUnit unit, Cell moveTarget);
        public void SendMeleeAttackCommand(BaseUnit unit, BaseUnit attackTarget, Cell from);
        public void SendRangeAttackCommand(BaseUnit unit, BaseUnit attackTarget);
        public void SendAbilityExecuteCommand(BaseUnit caster, Cell target, BaseAbility ability);
        public void SendSkipTurnCommand(BaseUnit unit);
    }
}