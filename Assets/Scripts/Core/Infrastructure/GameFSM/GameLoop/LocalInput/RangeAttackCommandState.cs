using System.Runtime.InteropServices.ComTypes;
using Core.Logic;
using Core.Logic.Grid;
using Core.Services;
using Core.Unit;
using Core.Unit.Component;
using UnityEngine;
using Utils.TypeBasedFSM;
using Cursor = Core.Logic.Cursor;

namespace Core.Infrastructure.GameFSM.GameLoop.LocalInput
{
    public class RangeAttackCommandState : IPayloadedState<Cell>, IParameterState<bool>
    {
        private readonly AbstractGrid _grid;
        private readonly Cursor _cursor;
        private readonly IUnitSelector _unitSelector;
        private readonly ITurnResolver _turn;

        private BaseUnit _target;
        private bool _canPerformAttack;

        public bool Parameter => _canPerformAttack;

        public RangeAttackCommandState(AbstractGrid grid, Cursor cursor, IUnitSelector unitSelector, ITurnResolver turn)
        {
            _grid = grid;
            _cursor = cursor;
            _unitSelector = unitSelector;
            _turn = turn;
        }

        public void Enter(Cell focusedCell)
        {
            _cursor.State = CursorState.RangeAttack;

            _grid.Units.TryGetUnit(focusedCell, out _target);

            UnitRangeAttack rangeAttackComponent = _turn.ActiveUnit.GetComponent<UnitRangeAttack>();

            if (rangeAttackComponent.CanAttack(_turn.ActiveUnit.CurrentCell, focusedCell, _target,
                    out BaseUnit obstacle))
            {
                _unitSelector.RenderOutline(new BaseUnit[] { _target }, SelectorSettings.Red);
                _canPerformAttack = true;
            }
            else
            {
                _unitSelector.RenderOutline(new BaseUnit[] {_target}, SelectorSettings.Unreachable);
                _unitSelector.RenderOutline(new BaseUnit[] {obstacle}, SelectorSettings.Red);
                _canPerformAttack = false;
            }
        }

        public void Exit()
        {
            _unitSelector.RenderOffAllOutlines();
            _target = null;
        }
    }
}