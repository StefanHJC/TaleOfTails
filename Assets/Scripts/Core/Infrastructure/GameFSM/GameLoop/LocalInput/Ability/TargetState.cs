using System.Collections.Generic;
using System.Linq;
using Core.Database;
using Core.Logic;
using Core.Logic.Grid;
using Core.Logic.Grid.StateMachine;
using Core.Services;
using Core.Services.Input;
using Core.Unit;
using Core.Unit.Ability;
using Utils.TypeBasedFSM;

namespace Core.Infrastructure.GameFSM.GameLoop.LocalInput
{
    public class TargetState : IPayloadedState<BaseAbility>, IParameterState<TargetAbility>
    {
        private readonly AbstractGrid _grid;
        private readonly IInputService _input;
        private readonly IUnitSelector _selector;

        private IEnumerable<Cell> _actionableCells;
        private TargetAbility _ability;
        private BaseUnit _focusedUnit;

        public TargetAbility Parameter => _ability;

        public TargetState(AbstractGrid grid, IInputService input, IUnitSelector selector)
        {
            _grid = grid;
            _input = input;
            _selector = selector;
        }

        public void Enter(BaseAbility payload)
        {
            _ability = (TargetAbility)payload;
            _actionableCells = _ability.GetActionableTargets();

            _grid.StateMachine.Enter<HighlightÑoncreteCellsState, IEnumerable<Cell>>(_actionableCells);
            _input.CellFocused += OnCellFocused;
        }

        public void Exit()
        {
            if (_focusedUnit != null)
                _selector.RenderOffOutline(_focusedUnit);
        }

        public bool ValidateCommand(Cell focusedCell) => focusedCell != null && _actionableCells.Contains(focusedCell);

        private void OnCellFocused(Cell focused)
        {
            if (focused != null && _focusedUnit != null)
                _selector.RenderOffOutline(_focusedUnit);

            if (_actionableCells.Contains(focused))
            {
                if (_grid.Units.TryGetUnit(focused, out BaseUnit focusedUnit))
                {
                    _focusedUnit = focusedUnit;

                    if (_ability.Id == AbilityId.Heal)
                    {
                        _selector.RenderOutline(focusedUnit, SelectorSettings.Green);
                    }
                    else
                    {
                        _selector.RenderOutline(focusedUnit, SelectorSettings.Red);
                    }
                }
            }
        }
    }
}