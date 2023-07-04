namespace Core.Unit.Command
{
    public readonly struct SkipTurnCommand : IUnitCommand
    {
        public CommandTypeId CommandType => CommandTypeId.SkipTurn;
    }
}