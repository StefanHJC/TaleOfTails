using Core.Logic;

namespace Core.Unit.Command
{
    public readonly struct RotateCommand : IUnitCommand
    {
        private readonly Cell _target;

        public CommandTypeId CommandType => CommandTypeId.Rotate;
        public Cell Target => _target;

        public RotateCommand(Cell to)
        {
            _target = to;
        }
    }
}