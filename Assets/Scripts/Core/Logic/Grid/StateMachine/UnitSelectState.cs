using Core.Services;
using Core.Services.Input;
using Core.Unit;
using UnityEngine;

namespace Core.Logic.Grid.StateMachine
{
    public class UnitSelectState : IGridState
    {
        private readonly IInputService _input;
        private readonly ICellHighlighter _highlighter;
        private readonly IUnitSelector _selector;
        private readonly UnitsOnGrid _unitsOnGrid;
        private readonly GridStateMachine _stateMachine;

        private Cell _currentFocus;

        public UnitSelectState(IInputService input, ICellHighlighter highlighter, IUnitSelector selector, UnitsOnGrid units, GridStateMachine stateMachine)
        {
            _input = input;
            _highlighter = highlighter;
            _selector = selector;
            _unitsOnGrid = units;
            _stateMachine = stateMachine;
        }

        public void Enter() => SubscribeInputEvents();

        public void Exit() => UnsubscribeInputEvents();

        private void SubscribeInputEvents()
        {
            _input.CellFocused += OnCellFocused;
            _input.SelectButtonPressed += OnSelectButtonPressed;
        }

        private void UnsubscribeInputEvents()
        {
            _input.CellFocused -= OnCellFocused;
            _input.SelectButtonPressed -= OnSelectButtonPressed;
        }

        private void OnSelectButtonPressed()
        {/*
            if (IsUnitSelected())
                _stateMachine.Enter<HighlightActionableCellsState>();*/
        }

        private bool IsUnitSelected()
        {
            if (_currentFocus != null && _unitsOnGrid.TryGetUnit(from: _currentFocus, out BaseUnit unit))
            {
                _input.SelectedUnit = unit;
                _selector.Select(_input.SelectedUnit);

                return true;
            }
            return false;
        }

        private void OnCellFocused(Cell focused)
        {
            if (focused == null)
            {
                OnEmptySpaceFocused();
            }
            else
            {
                _currentFocus = focused;
                _highlighter.Highlight(focused);
            }
        }

        private void OnEmptySpaceFocused()
        {
            _highlighter.Dehighlight(_currentFocus);
            _currentFocus = null;
        }
    }
}