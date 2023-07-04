namespace Core.Unit.Command
{
    public interface IAttackCommandHandler : IUnitCommandHandler
    {
        public new CommandTypeId HandlerType => CommandTypeId.Attack;
        public AttackTypeId AttackType { get; }
    }
}