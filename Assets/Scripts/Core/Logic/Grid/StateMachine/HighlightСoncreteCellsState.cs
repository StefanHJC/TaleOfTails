using System.Collections.Generic;
using Core.Services;
using Utils.TypeBasedFSM;

namespace Core.Logic.Grid.StateMachine
{
    public class HighlightСoncreteCellsState : IPayloadedState<IEnumerable<Cell>>
    {
        private readonly ICellHighlighter _highlighter;
        
        private IEnumerable<Cell> _cellsToHighlight;

        public HighlightСoncreteCellsState(ICellHighlighter highlighter)
        {
            _highlighter = highlighter;
        }

        public void Enter(IEnumerable<Cell> payload)
        {
            _cellsToHighlight = payload;
            _highlighter.Reset();
            _highlighter.Highlight(_cellsToHighlight);
        }

        public void Exit() => _highlighter.Dehighlight(_cellsToHighlight);
    }
}