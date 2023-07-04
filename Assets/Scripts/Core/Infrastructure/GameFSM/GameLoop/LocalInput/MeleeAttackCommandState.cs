using System.Collections;
using Core.Logic;
using System.Collections.Generic;
using System.Linq;
using Core.Logic.Grid;
using Core.Services;
using Core.Services.Input;
using Core.Unit;
using UnityEngine;
using Utils.TypeBasedFSM;
using Cursor = Core.Logic.Cursor;
using Vector3 = UnityEngine.Vector3;

namespace Core.Infrastructure.GameFSM.GameLoop.LocalInput
{
    public class MeleeAttackCommandState : IPayloadedState<Cell>, IParameterState<Cell>
    {
        private readonly AbstractGrid _grid;
        private readonly Cursor _cursor;
        private readonly IInputService _input;
        private readonly IPathRenderer _pathRenderer;
        private readonly IUnitActionsResolver _unitActions;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IUnitSelector _unitSelector;

        private BaseUnit _target;
        private Cell _meleeAttackStartCell;
        private Coroutine _tickRoutine;

        private const float _eps = 1e-1f;

        public Cell Parameter
        {
            get => _meleeAttackStartCell;
            set
            {
                if (value != _meleeAttackStartCell)
                {
                    _meleeAttackStartCell = value;
                    _pathRenderer.RenderPath(_unitActions.ReachableCells[_meleeAttackStartCell]);
                    SetCursorRotation(_meleeAttackStartCell);
                }
            }
        }

        public MeleeAttackCommandState(AbstractGrid grid, IInputService input,
            IPathRenderer pathRenderer, IUnitActionsResolver unitActions, Cursor cursor,
            ICoroutineRunner coroutineRunner, IUnitSelector unitSelector)
        {
            _grid = grid;
            _input = input;
            _pathRenderer = pathRenderer;
            _unitActions = unitActions;
            _cursor = cursor;
            _coroutineRunner = coroutineRunner;
            _unitSelector = unitSelector;
        }

        public void Enter(Cell focusedCell)
        {
            _cursor.State = CursorState.MeleeAttack;
            _grid.Units.TryGetUnit(focusedCell, out _target);
            _unitSelector.RenderOutline(_target, SelectorSettings.Red);
            _tickRoutine = _coroutineRunner.StartCoroutine(TickRoutine());
        }

        public void Exit()
        {
            _coroutineRunner.StopCoroutine(_tickRoutine);
            _unitSelector.RenderOffOutline(_target);
            ResetCursorRotation();
            _target = null;
        }

        private IEnumerator TickRoutine()
        {
            while (true)
            {
                HandleMeleeAttackStartCell();
                yield return null;
            }
        }

        private void HandleMeleeAttackStartCell()
        {
            if (_input.FocusedCell == null) // TODO make _input.FocusedCell always NOT null
                return;

            IEnumerable<Cell> availableAttackStartCells = GetReachableCellsForAttackStartCell(_input.FocusedCell);

            Parameter = GetNearestToMouseCell(availableAttackStartCells);
        }


        private Cell GetNearestToMouseCell(IEnumerable<Cell> availableAttackStartCells)
        {
            Cell nearestAttackStartCell = availableAttackStartCells.First(); //bug here

            foreach (Cell cell in availableAttackStartCells)
                if (CompareCellsPositionToMouseOnGridPostion(nearestAttackStartCell, cell))
                    nearestAttackStartCell = cell;

            return nearestAttackStartCell;
        }

        private void SetCursorRotation(Cell nearestAttackStartCell)
        {
            var targetRotation =
                Quaternion.LookRotation(nearestAttackStartCell.transform.position -
                                        _input.FocusedCell.transform.position);

            _cursor.Rotation =
                targetRotation.eulerAngles.y - 180 - Camera.main.transform.rotation.eulerAngles.y;
        }

        private void ResetCursorRotation() => _cursor.Rotation = 0;


        private bool CompareCellsPositionToMouseOnGridPostion(Cell a, Cell b) =>
            Vector3.Distance(_input.PointerPositionOnGrid, b.transform.position) -
            Vector3.Distance(_input.PointerPositionOnGrid, a.transform.position) < _eps;

        private IEnumerable<Cell> GetReachableCellsForAttackStartCell(Cell focusedCell) =>
            _grid
                .GetCellNeighbours(focusedCell)
                .Intersect(_unitActions.ReachableCells.Keys);
    }
}