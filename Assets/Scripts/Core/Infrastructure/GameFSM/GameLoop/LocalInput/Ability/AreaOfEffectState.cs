using System.Collections.Generic;
using Core.Logic;
using Core.Logic.Grid;
using Core.Logic.Grid.StateMachine;
using Core.Services;
using Core.Services.Input;
using Core.Unit;
using Core.Unit.Ability;
using Utils.TypeBasedFSM;
using Zenject;

namespace Core.Infrastructure.GameFSM.GameLoop.LocalInput
{
    public class AreaOfEffectState : IPayloadedState<BaseAbility>, IParameterState<AreaOfEffectAbility>
    {
        private readonly AbstractGrid _grid;
        private readonly IUnitSelector _selector;
        private readonly IInputService _input;
        
        private AreaOfEffectAbility _selectedAbility;

        public AreaOfEffectAbility Parameter => _selectedAbility;

        [Inject]
        public AreaOfEffectState(AbstractGrid grid, IUnitSelector selector, IInputService input)
        {
            _grid = grid;
            _selector = selector;
            _input = input;
        }

        public void Enter(BaseAbility payload)
        {
            _selectedAbility = (AreaOfEffectAbility)payload;

            if (_input.FocusedCell != null)
                _grid.StateMachine.Enter<HighlightÑoncreteCellsState, IEnumerable<Cell>>(
                    _selectedAbility.GetAffectedCells(_input.FocusedCell));
            else
                _grid.StateMachine.Enter<InputLockState>();

            _input.CellFocused += OnCellFocused;
        }

        public void Exit()
        {
            _input.CellFocused -= OnCellFocused;

            _selector.RenderOffAllOutlines();
        }

        public bool ValidateCommand(Cell focusedCell) => focusedCell != null;

        private void OnCellFocused(Cell focused)
        {
            if (focused == null)
                return;

            _grid.StateMachine.Enter<HighlightÑoncreteCellsState, IEnumerable<Cell>>(_selectedAbility.GetAffectedCells(_input.FocusedCell));
            _selector.RenderOffAllOutlines();
            _selector.RenderOutline(GetAffectedUnits(focused), SelectorSettings.Red);
        }

        private IEnumerable<BaseUnit> GetAffectedUnits(Cell focused)
        {
            List<BaseUnit> affectedUnits = new List<BaseUnit>();

            foreach (Cell affectedCell in _selectedAbility.GetAffectedCells(focused))
                if (_grid.Units.TryGetUnit(affectedCell, out BaseUnit affectedUnit))
                    affectedUnits.Add(affectedUnit);

            return affectedUnits;
        }
    }
}