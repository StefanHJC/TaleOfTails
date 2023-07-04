using System;
using System.Collections.Generic;
using System.Linq;
using Core.Database;
using Core.Logic;
using Core.Logic.Grid;
using Core.Logic.Projectile;
using Core.UI.HUD;
using Core.Unit.Component;
using FMODUnity;
using UnityEngine;
using Zenject;

namespace Core.Unit.Ability
{
    public class FrostArrow : TargetAbility
    {
        [SerializeField] private EventReference _castSfx;
        [SerializeField] private EventReference _performSfx;
        [SerializeField] private EventReference _hitSFX;
        [SerializeField] private EventReference _flightSFX;
        [SerializeField] private TargetProjectile _projectile;

        private ITurnResolver _turnResolver;
        private AbstractGrid _grid;
        private BaseUnit _caster;
        private AbilityStaticData _config;
        private BaseUnit _target;
        private Transform _projectileParent;
        private int _cooldown;

        public override AbilityId Id => AbilityId.FrostArrow;
        public override BaseUnit Caster => _caster;
        public override int Cooldown => _cooldown;

        public event Action Executed;

        [Inject]
        public void Construct(AbstractGrid grid, BaseUnit caster, AbilityStaticData config, ITurnResolver turnResolver)
        {
            _grid = grid;
            _caster = caster;
            _config = config;
            _turnResolver = turnResolver;
            _cooldown = 0;

            _turnResolver.RoundStarted += ReduceCooldown;
        }

        public override void Perform(Cell target, Action onExecuted)
        {
            Executed = onExecuted;

            GetRangeAttackInfo();
            _caster.GetComponent<BaseUnitAnimator>().PlayAbility1();
            _caster.Sound.PlayOnce(_castSfx, _caster.transform.position);

            if (_grid.Units.TryGetUnit(target, out BaseUnit targetUnit))
                _target = targetUnit;

            _cooldown = _config.Cooldown;
        }

        public override void OnAnimatorPerform()
        {
            ThrowAbility();
            _caster.Sound.PlayOnce(_performSfx, _caster.transform.position);
        }

        public override IEnumerable<Cell> GetActionableTargets() =>
            _grid
                .Units
                .List
                .Where(unit => unit.TeamId != _caster.TeamId)
                .Select(unit => unit.CurrentCell);

        private void ThrowAbility()
        {
            _projectileParent.LookAt(_target.Torso);

            TargetProjectile projectile = Instantiate(_projectile, _projectileParent.transform.position,
                _projectileParent.rotation);
            projectile.Construct(_flightSFX, _hitSFX, _caster);
            projectile.TargetReached += OnTargetHit;
        }

        private void OnTargetHit(BaseUnit hittedUnit)
        {
            hittedUnit.GetComponent<UnitHealth>().TakeDamage(_config.Value, Executed);
            hittedUnit.GetComponent<EffectHandler>().ReduceSpeed(1);
            hittedUnit.HealthRenderer.RenderStatusMessage("Обездвижен", HealthRendererSettings.Effect);
        }

        private void GetRangeAttackInfo()
        {
            UnitRangeAttack rangeAttackComponent = _caster.GetComponent<UnitRangeAttack>();
            _projectileParent = rangeAttackComponent.ProjectileParent;
        }

        private void ReduceCooldown() => Mathf.Clamp(--_cooldown, 0, Int32.MaxValue);
    }
}