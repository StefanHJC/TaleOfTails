using Core.Logic;
using Core.Logic.Grid;

namespace Core.Services.Pathfinder
{
    public interface IPathfinderService
    {
        public Path CachedPath { get; }

        public Path GetPath(Cell from, Cell to);

        public int GetDistance(Cell from, Cell to);
    }
}