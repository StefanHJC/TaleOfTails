using Core.Services;
using Utils.TypeBasedFSM;

namespace Core.Logic.Grid.StateMachine
{
    public class InputLockState : IState
    {
        private readonly IPathRenderer _pathRenderer;
        private readonly IUnitSelector _unitSelector;
        private readonly ICellHighlighter _highlighter;

        public InputLockState(IPathRenderer pathRenderer, ICellHighlighter highlighter, IUnitSelector unitSelector)
        {
            _pathRenderer = pathRenderer;
            _highlighter = highlighter;
            _unitSelector = unitSelector;
        }

        public void Enter()
        {
            _pathRenderer.RenderOff();
            _highlighter.Reset();
            _unitSelector.Deselect();
        }

        public void Exit()
        {
        }
    }
}