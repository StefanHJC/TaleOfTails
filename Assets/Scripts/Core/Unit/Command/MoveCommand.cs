using Core.Logic;

namespace Core.Unit.Command
{
    public readonly struct MoveCommand : IUnitCommand
    {
        private readonly Cell _target;

        public CommandTypeId CommandType => CommandTypeId.Move;
        public Cell Target => _target;

        public MoveCommand(Cell to)
        {
            _target = to;
        }
    }
}