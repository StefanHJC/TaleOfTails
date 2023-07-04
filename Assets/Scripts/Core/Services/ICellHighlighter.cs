using System.Collections.Generic;
using Core.Logic;
using UnityEngine;

namespace Core.Services
{
    public interface ICellHighlighter
    {
        public IReadOnlyList<GameObject> HighlightedCells { get; }
        public void Highlight(Cell cell);
        public void Highlight(IEnumerable<Cell> cells);
        public void Dehighlight(IEnumerable<Cell> cells);
        public void Dehighlight(Cell cell);
        public void Reset();

        public void ClearAll();
    }
}