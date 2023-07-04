using System.Collections.Generic;
using Core.Services.Pathfinder;
using Core.Unit;
using Core.Unit.Command;

namespace Core.Logic
{
    public interface IUnitActionsResolver
    {
        public IReadOnlyDictionary<Cell, Path> ReachableCells { get; }
        public IReadOnlyList<(BaseUnit target, AttackTypeId attackType)> AvailableAttacks { get; }
        public IEnumerable<Cell> ActionableCells { get; }
        public IEnumerable<Cell> AttackableCells { get; }

        public Dictionary<Cell, Path> GetReachableCells(BaseUnit targetUnit, int searchDepth);

        public List<(BaseUnit target, AttackTypeId attackType)> GetAvailableAttacks(BaseUnit targetUnit);
    }
}