namespace Core.Unit.Command
{
    public readonly struct AttackCommand : IUnitCommand
    {
        private readonly BaseUnit _target;
        private readonly AttackTypeId _attackType;

        public CommandTypeId CommandType => CommandTypeId.Attack;
        public AttackTypeId AttackType => _attackType;
        public BaseUnit Target => _target;

        public AttackCommand(BaseUnit target, AttackTypeId attacktType)
        {
            _target = target;
            _attackType = attacktType;
        }
    }
}