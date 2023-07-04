using System;

namespace Core.Unit.Command
{
    public interface IUnitCommandHandler
    {
        public CommandTypeId HandlerType { get; }

        public event Action Executed;

        public void Execute(IUnitCommand command, Action onExecuted);
    }
}