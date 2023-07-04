using System;
using System.Collections.Generic;

namespace Core.Unit.Command
{
    public interface ICommandReceiver
    {
        public IReadOnlyList<IUnitCommand> CommandQueue { get; }
        public IReadOnlyList<IUnitCommandHandler> CommandHandlers { get; }


        public event Action<ICommandReceiver> CommandExecuted;

        public void ExecuteCommand(Queue<IUnitCommand> command);
    }
}