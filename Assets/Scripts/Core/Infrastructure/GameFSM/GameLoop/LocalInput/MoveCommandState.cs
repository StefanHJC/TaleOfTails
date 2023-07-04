using Core.Logic;
using Core.Logic.Grid;
using Core.Services;
using Core.Services.Input;
using Utils.TypeBasedFSM;

namespace Core.Infrastructure.GameFSM.GameLoop.LocalInput
{
    public class MoveCommandState : IState
    {
        private readonly AbstractGrid _grid;
        private readonly Cursor _cursor;
        private readonly IInputService _input;
        private readonly IPathRenderer _pathRenderer;
        private readonly IUnitActionsResolver _unitActions;

        public MoveCommandState(AbstractGrid grid, Cursor cursor, IInputService input, 
            IPathRenderer pathRenderer, IUnitActionsResolver unitActions)
        {
            _grid = grid;
            _cursor = cursor;
            _input = input;
            _pathRenderer = pathRenderer;
            _unitActions = unitActions;
        }

        public void Enter()
        {
            _cursor.State = CursorState.Move;

            _input.CellFocused += OnCellFocused;
        }

        public void Exit()
        {
            _input.CellFocused -= OnCellFocused;
            _pathRenderer.RenderOff();
        }

        private void OnCellFocused(Cell focused)
        {
            if (focused != null && _unitActions.ReachableCells.ContainsKey(focused))
                _pathRenderer.RenderPath(_unitActions.ReachableCells[focused]);
        }
    }
}