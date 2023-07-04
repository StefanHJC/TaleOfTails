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
    public class StunBombArrow : AreaOfEffectAbility
    {
        [SerializeField] private EventReference _castSFX;
        [SerializeField] private EventReference _flightSFX;
        [SerializeField] private EventReference _hitSFX;
        [SerializeField] private AoeProjectile _projectile;

        private ITurnResolver _turnResolver;
        private BaseUnit _caster;
        private AbstractGrid _grid;
        private AbilityStaticData _config;
        private Transform _projectileParent;
        private Cell _target;
        private List<BaseUnit> _afectedUnits;
        private int _cooldown;

        public override AbilityId Id => AbilityId.StunBombArrow;
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
            Executed = onExecuted;
            GetRangeAttackInfo();

            _target = target;
            _caster.GetComponent<BaseUnitAnimator>().PlayAbility2();
            _caster.Sound.PlayOnce(_castSFX, _caster.transform.position);
            ApplyEffect();

            _cooldown = _config.Cooldown;
        }

        public override void OnAnimatorPerform()
        {
            ThrowAoeAbility();
        }

        public override IEnumerable<Cell> GetAffectedCells(Cell target)
        {
            var firstAffectedCell = _grid.GetCellNeighbours(target).First();
            var secondAffectedCell = _grid.GetCellNeighbours(firstAffectedCell).First(cell => 
                _grid.GetCellNeighbours(cell).Contains(firstAffectedCell) &&
                _grid.GetCellNeighbours(cell).Contains(target));
            
            List<Cell> affectedCells = new List<Cell> {firstAffectedCell, secondAffectedCell, target};

            return affectedCells;
        }

        private void ThrowAoeAbility()
        {
            _projectileParent.LookAt(new Vector3(_target.transform.position.x, _target.transform.position.y + .5f, _target.transform.position.z));
            AoeProjectile projectile = Instantiate(_projectile, _projectileParent.transform.position, _projectileParent.rotation);
            projectile.Construct(_flightSFX, _hitSFX, _caster);
            projectile.TargetReached += OnExplosion;
        }

        private void OnExplosion()
        {
            foreach (BaseUnit unit in _afectedUnits)
            {
                unit.GetComponent<UnitHealth>().TakeDamage(_config.Value);
                unit.HealthRenderer.RenderStatusMessage("Обездвижен", HealthRendererSettings.Effect);
            }

            Executed?.Invoke();
        }

        private void ApplyEffect()
        {
            var afectedCells = GetAffectedCells(_target);
            _afectedUnits = new List<BaseUnit>();

            foreach (Cell cell in afectedCells)
            {
                if (_grid.Units.TryGetUnit(cell, out BaseUnit affectedUnit))
                {
                    _afectedUnits.Add(affectedUnit);
                    affectedUnit.GetComponent<EffectHandler>().ReduceSpeed(durationInRounds: 1);
                }
            }
        }

        private void GetRangeAttackInfo()
        {
            UnitRangeAttack rangeAttackComponent = _caster.GetComponent<UnitRangeAttack>();
            _projectileParent = rangeAttackComponent.ProjectileParent;
        }

        private void ReduceCooldown()
        {
            _cooldown--;
            Mathf.Clamp(_cooldown, 0, Int32.MaxValue);
        }
    }
}