using System.Collections.Generic;
using System.Linq;
using Core.Logic;

namespace Core.Services.Pathfinder
{
    public readonly struct Path
    {
        public readonly List<Cell> _list;

        public Cell Start => _list.First();
        public Cell End => _list.Last();
        public IReadOnlyList<Cell> List => _list;

        public Path(List<Cell> list)
        {
            _list = list;
        }
    }
}