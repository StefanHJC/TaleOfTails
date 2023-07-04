using Core.Logic;
using Core.Unit.Ability;

namespace Core.Unit.Command
{
    public readonly struct AbilityCommand : IUnitCommand
    {
        private readonly Cell _target;
        private readonly BaseAbility _ability;
        private readonly BaseUnit _caster;

        public CommandTypeId CommandType => CommandTypeId.UseAbility;
        public Cell Target => _target;
        public BaseAbility Ability => _ability;
        public BaseUnit Caster => _caster;

        public AbilityCommand(Cell target, BaseAbility ability, BaseUnit caster)
        {
            _target = target;
            _ability = ability;
            _caster = caster;
        }
    }
}