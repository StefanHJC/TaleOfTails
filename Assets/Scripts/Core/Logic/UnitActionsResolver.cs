using System.Collections.Generic;
using System.Linq;
using Core.Logic.Grid;
using Core.Services.Pathfinder;
using Core.Unit;
using Core.Unit.Command;
using Core.Unit.Component;
using Zenject;

namespace Core.Logic
{
    public class UnitActionsResolver : IUnitActionsResolver
    {
        private readonly AbstractGrid _grid;
        private readonly IPathfinderService _pathfinder;
        private readonly ITurnResolver _turnResolver;

        private bool _isRanged; // TODO rework
        private Dictionary<Cell, Path> _reachableCells = new Dictionary<Cell, Path>();
        private List<(BaseUnit target, AttackTypeId attackType)> _availableAttacks = new List<(BaseUnit target, AttackTypeId attackType)>();

        public IReadOnlyDictionary<Cell, Path> ReachableCells => _reachableCells;
        public IReadOnlyList<(BaseUnit target, AttackTypeId attackType)> AvailableAttacks => _availableAttacks;
        public IEnumerable<Cell> ActionableCells => GetActionableCells();
        public IEnumerable<Cell> AttackableCells => GetAttackableCells();

        [Inject]
        public UnitActionsResolver(AbstractGrid grid, IPathfinderService pathfinder, ITurnResolver turnResolver)
        {
            _grid = grid;
            _pathfinder = pathfinder;
            _turnResolver = turnResolver;

            _turnResolver.TurnStarted += OnTurn;
        }

        public Dictionary<Cell, Path> GetReachableCells(BaseUnit targetUnit, int searchDepth)
        {
            Dictionary<Cell, Path> reachableCells = new Dictionary<Cell, Path>();
            List<Cell> cells = _grid.GetCellsInRange(targetUnit.CurrentCell, searchDepth);

            foreach (Cell cell in cells)
            {
                Path path;
                path = _pathfinder.GetPath(targetUnit.CurrentCell, cell);

                if (path.List.Count > 0)
                {
                    reachableCells.Add(cell, path);
                }
            }
            
            return reachableCells;
        }

        public List<(BaseUnit target, AttackTypeId attackType)> GetAvailableAttacks(BaseUnit targetUnit) // temp
        {
            List<(BaseUnit, AttackTypeId)> availableAttacks = new List<(BaseUnit, AttackTypeId)>();

            foreach (BaseUnit unit in _turnResolver.Units)
            {
                if (unit.TeamId != _turnResolver.ActiveUnit.TeamId &&
                    _grid.GetCellNeighbours(unit.CurrentCell).Any(cell => ReachableCells.ContainsKey(cell)))
                {
                    availableAttacks.Add((unit, AttackTypeId.Melee)); // temp
                }
            }

            if (_isRanged)
            {
                if (HasEnemyOnNeighbourCell(targetUnit))
                    return availableAttacks;

                foreach (BaseUnit unit in _turnResolver.Units) // TODO rework
                {
                    if (unit.TeamId != _turnResolver.ActiveUnit.TeamId)
                        availableAttacks.Add((unit, AttackTypeId.Ranged)); // super temp
                }
            }
            return availableAttacks;
        }

        private void OnTurn()
        {
            _isRanged = _turnResolver.ActiveUnit.TryGetComponent<UnitRangeAttack>(out var _);
            _reachableCells = GetReachableCells(_turnResolver.ActiveUnit, _turnResolver.ActiveUnit.ActionPoints);
            _availableAttacks = GetAvailableAttacks(_turnResolver.ActiveUnit);
        }

        private bool HasEnemyOnNeighbourCell(BaseUnit targetUnit) =>
                _grid
                .GetCellNeighbours(targetUnit.CurrentCell)
                .Any(cell => _grid.Units.TryGetUnit(cell, out BaseUnit unit) && unit.TeamId != _turnResolver.ActiveUnit.TeamId);

        private IEnumerable<Cell> GetActionableCells() =>
                ReachableCells.Keys
                .Union(GetAttackableCells()).Distinct();

        private IEnumerable<Cell> GetAttackableCells() =>
                AvailableAttacks
                .Where(attack => attack.target.TeamId != _turnResolver.ActiveUnit.TeamId)
                .Select(attack => attack.target.CurrentCell)
                .Distinct();
    }
}