using System;
using System.Collections.Generic;
using Core.Database;
using Core.Logic;
using Core.Logic.Grid;
using Core.Logic.Projectile;
using Core.Services;
using Core.Unit.Component;
using FMODUnity;
using UnityEngine;
using Zenject;

namespace Core.Unit.Ability
{
    public class FireballAbility : AreaOfEffectAbility
    {
        [SerializeField] private AoeProjectile _fireballVFX;
        [SerializeField] private EventReference _castSFX;
        [SerializeField] private EventReference _performedSFX;
        [SerializeField] private EventReference _flightSFX;
        [SerializeField] private EventReference _hitSFX;

        private ISoundService _sound;
        private ITurnResolver _turnResolver;
        private BaseUnit _caster;
        private AbstractGrid _grid;
        private Cell _target;
        private AbilityStaticData _config;
        private CameraShaker _cameraShaker;
        private int _cooldown;

        public override BaseUnit Caster => _caster;
        public override int Cooldown => _cooldown;
        public override AbilityId Id => AbilityId.Fireball;
        public event Action Performed;

        [Inject]
        public void Construct(BaseUnit caster, AbstractGrid grid, AbilityStaticData config, ISoundService sound, ITurnResolver turnResolver)
        {
            _caster = caster;
            _grid = grid;
            _sound = sound;
            _config = config;
            _turnResolver = turnResolver;
            _cooldown = 0;

            _turnResolver.RoundStarted += ReduceCooldown;
            _cameraShaker = FindObjectOfType<CameraShaker>();
        }

        public override void Perform(Cell target, Action onExecuted)
        {
            _caster.GetComponent<BaseUnitAnimator>().PlayAbility1();
            _caster.Sound.PlayOnce(_castSFX, _caster.transform.position);
            _target = target;
            Performed = onExecuted;
            _cooldown = _config.Cooldown;
        }

        private void OnExplosion()
        {
            ApplyDamage(_target);
            _cameraShaker?.ShakeCamera(5, .4f);
            Performed?.Invoke();
        }

        private void ApplyDamage(Cell target)
        {
            IEnumerable<Cell> affectedCells = GetAffectedCells(target);
            List<BaseUnit> affectedUnits = new List<BaseUnit>();

            foreach (Cell cell in affectedCells)
                if (_grid.Units.TryGetUnit(cell, out BaseUnit unit) && cell != target)
                    affectedUnits.Add(unit);

            foreach (BaseUnit unit in affectedUnits)
            {
                unit.GetComponent<UnitHealth>().TakeDamage(_config.Value / 2);
            }

            if (_grid.Units.TryGetUnit(target, out BaseUnit centralUnit))
                centralUnit.GetComponent<UnitHealth>().TakeDamage(_config.Value);
        }

        public override IEnumerable<Cell> GetAffectedCells(Cell target)
        {
            List<Cell> affectedCells = _grid.GetCellNeighbours(target);
            affectedCells.Add(target);

            return affectedCells;
        }

        public override void OnAnimatorPerform()
        {
            _caster.Sound.PlayOnce(_performedSFX, _caster.transform.position);

            AoeProjectile projectile = Instantiate(_fireballVFX,
                new Vector3(_grid.GridCenter.transform.position.x + 10, _grid.GridCenter.transform.position.y + 25, _grid.GridCenter.transform.position.z - 10),
                Quaternion.identity);

            projectile.Construct(_flightSFX, _hitSFX, _caster);
            projectile.TargetReached += OnExplosion;
            projectile.transform.LookAt(_target.transform);
            _cameraShaker?.ShakeCamera(.5f, 1f);
        }

        private void ReduceCooldown() => Mathf.Clamp(--_cooldown, 0, Int32.MaxValue);
    }
}