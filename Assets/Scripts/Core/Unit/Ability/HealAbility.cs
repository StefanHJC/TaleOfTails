using System;
using System.Collections.Generic;
using System.Linq;
using Core.Database;
using Core.Logic;
using Core.Logic.Grid;
using Core.Unit.Component;
using FMODUnity;
using UnityEngine;
using Zenject;

namespace Core.Unit.Ability
{
    public class HealAbility : TargetAbility
    {
        [SerializeField] private EventReference _castSfx;
        [SerializeField] private EventReference _performSfx;
        [SerializeField] private GameObject _healVfx;

        private ITurnResolver _turnResolver;
        private BaseUnit _caster;
        private AbstractGrid _grid;
        private Cell _target;
        private AbilityStaticData _config;
        private int _cooldown;

        public override AbilityId Id => AbilityId.Heal;
        public override BaseUnit Caster => _caster;
        public override int Cooldown => _cooldown;

        public event Action Executed;

        [Inject]
        public void Construct(BaseUnit caster, AbstractGrid grid, AbilityStaticData config, ITurnResolver turnResolver)
        {
            _caster = caster;
            _grid = grid;
            _config = config;
            _turnResolver = turnResolver;
            _cooldown = 0;

            _turnResolver.RoundStarted += ReduceCooldown;
        }

        public override void Perform(Cell target, Action onExecuted)
        {
            _target = target;
            Executed = onExecuted;
            _caster.GetComponent<BaseUnitAnimator>().PlayAbility2();

            _cooldown = _config.Cooldown;
        }

        public override IEnumerable<Cell> GetActionableTargets() => 
            _grid
                .Units
                .List
                .Where(unit => unit.TeamId == _caster.TeamId && unit.Health.Health != unit.Config.MaxHealth)
                .Select(unit => unit.CurrentCell);

        public override void OnAnimatorPerform()
        {
            if (_grid.Units.TryGetUnit(_target, out BaseUnit unit))
                unit.GetComponent<UnitHealth>().TakeHeal(_config.Value);
            Instantiate(_healVfx, _target.transform.position, Quaternion.identity);
            Executed?.Invoke();
            _caster.Sound.PlayOnce(_performSfx, _caster.transform.position);
        }

        private void ReduceCooldown() => Mathf.Clamp(--_cooldown, 0, Int32.MaxValue);
    }
}