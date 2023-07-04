using System.Collections.Generic;
using System.Linq;
using Core.Infrastructure.GameFSM.GameLoop.LocalInput;
using Core.Logic;
using Core.Logic.Grid;
using Core.Logic.Grid.StateMachine;
using Core.Services;
using Core.Services.Input;
using Core.UI;
using Core.Unit;
using Core.Unit.Ability;
using Core.Unit.Component;
using ModestTree;
using UnityEngine;
using Utils.TypeBasedFSM;
using Zenject;
using Cursor = Core.Logic.Cursor;

namespace Core.Infrastructure.GameFSM.GameLoop
{
    public class LocalPlayerInputState : IPayloadedState<BaseUnit>
    {
        private readonly AbstractGrid _grid;
        private readonly Cursor _cursor;
        private readonly IInputService _input;
        private readonly ITurnResolver _turnResolver;
        private readonly IUnitActionsResolver _unitActions;
        private readonly ICommandInvoker _commandService;
        private readonly IGameFactory _gameFactory;

        private BaseUnit _currentUnit;
        private LocalInputStateMachine _localInputStateMachine;
        private bool _isCurrentUnitRanged;
        private UnitStatsCard _statCard;

        [Inject]
        public LocalPlayerInputState(IInputService input, AbstractGrid grid, ITurnResolver turnResolver, 
            IUnitActionsResolver unitActions, ICommandInvoker commandService, IGameFactory gameFactory, Cursor cursor)
        {
            _input = input;
            _grid = grid;
            _turnResolver = turnResolver;
            _commandService = commandService;
            _unitActions = unitActions;
            _cursor = cursor;
            _gameFactory = gameFactory;
        }

        public void Enter(BaseUnit activeUnit)
        {
            _localInputStateMachine = new LocalInputStateMachine(_gameFactory);
            SubscribeInputEvents();
            Init();
        }

        private void Init()
        {
            _currentUnit = _turnResolver.ActiveUnit;
            _isCurrentUnitRanged = _currentUnit.TryGetComponent<UnitRangeAttack>(out var _);
        }

        public void Exit()
        {
            UnsubscribeInputEvents();
            
            if (_statCard != null)
                Object.Destroy(_statCard);
        }

        public void OnAbilitySelected(BaseAbility ability) => _localInputStateMachine.Enter<AbilityInputState, BaseAbility>(ability);

        public void OnAbilityDeselected() => _localInputStateMachine.Enter<MoveCommandState>();

        private void SubscribeInputEvents()
        {
            _input.CellFocused += OnCellFocused;
            _input.CommandButtonPressed += OnCommandActionPerformed;
            _input.SelectButtonPressed += OnSelectActionPerformed;
        }

        private void UnsubscribeInputEvents()
        {
            _input.CellFocused -= OnCellFocused;
            _input.CommandButtonPressed -= OnCommandActionPerformed;
            _input.SelectButtonPressed -= OnSelectActionPerformed;
        }

        private void OnSelectActionPerformed()
        {
            if (_input.FocusedCell == null)
                return;

            if (_grid.Units.TryGetUnit(_input.FocusedCell, out BaseUnit focusedUnit))
            {
                _cursor.State = CursorState.InterfaceClick;
                _cursor.State = CursorState.Default;

                if(_statCard != null)
                {
                    UnitStatsCard buff = _statCard;
                    _statCard = _gameFactory.UI.CreateUnitStatsWindow(focusedUnit);
                    Object.Destroy(buff.gameObject);
                }
                else
                {
                    _statCard = _gameFactory.UI.CreateUnitStatsWindow(focusedUnit);
                }

                if (focusedUnit != _turnResolver.ActiveUnit)
                {
                    HighlightActionsFor(focusedUnit);
                }
            }
        }

        private void OnCommandActionPerformed()
        {
            if (ValidateCommand() == false)
                return;

            switch (_localInputStateMachine.CurrentState)
            {
                case MoveCommandState:
                {
                    _commandService.SendMoveCommand(_currentUnit, _input.FocusedCell);

                    break;
                }
                case RangeAttackCommandState state:
                {
                    if (state.Parameter == true)
                    {
                        _grid.Units.TryGetUnit(_input.FocusedCell, out BaseUnit target);
                        _commandService.SendRangeAttackCommand(_currentUnit, target);
                    }
                    else
                    {
                        return;
                    }
                    break;
                }
                case MeleeAttackCommandState state:
                {
                    _grid.Units.TryGetUnit(_input.FocusedCell, out BaseUnit target);
                    _commandService.SendMeleeAttackCommand(_currentUnit, target, state.Parameter);

                    break;
                }
                default:
                {
                    return;
                }
            }
            _localInputStateMachine.Enter<NullCommandState>();
        }

        private void OnCellFocused(Cell focusedCell) // TODO refactor
        {
            if (_statCard != null)
                Object.Destroy(_statCard.gameObject);

            if (_localInputStateMachine.CurrentState?.GetType() == typeof(AbilityInputState))
            {
                return;
            }
            DehighlightActions();

            if (focusedCell == null)
            {
                return;
            }
            if (_unitActions.ActionableCells.Contains(focusedCell) == false)
            {
                _localInputStateMachine.Enter<NullCommandState>();
            }
            if (_grid.Units.TryGetUnit(focusedCell, out BaseUnit focusedUnit) && focusedUnit.TeamId != _turnResolver.ActiveUnit.TeamId)
            {
                IEnumerable<Cell> availableAttackStartCells = GetReachableCellsForAttackStartCell(focusedCell); // TODO cache this

                if (_isCurrentUnitRanged)
                {
                    if (IsEnemyOnNeighbourCell())
                    {
                        _localInputStateMachine.Enter<MeleeAttackCommandState, Cell>(focusedCell);
                    }
                    else
                    {
                        _localInputStateMachine.Enter<RangeAttackCommandState, Cell>(focusedCell);
                    }
                }
                else if (availableAttackStartCells.IsEmpty() == false)
                {
                    _localInputStateMachine.Enter<MeleeAttackCommandState, Cell>(focusedCell);
                }
            }
            // else if (focusedUnit == _turnResolver.ActiveUnit)
            // {
            //     _localInputStateMachine.Enter<NullCommandState>();
            // }
            else
            {
                _localInputStateMachine.Enter<MoveCommandState>();
            }
        }

        private bool ValidateCommand()
        {
            IEnumerable<Cell> interactableCells =  _unitActions
                .AvailableAttacks
                .Select(attack => attack.target.CurrentCell)
                .Union(_unitActions.ReachableCells.Keys)
                .Distinct();

            return interactableCells.Contains(_input.FocusedCell);
        }

        private void HighlightActionsFor(BaseUnit focusedUnit)
        {
            if (focusedUnit == null)
                Debug.Log("XUY");

            IEnumerable<Cell> actionableCells = _unitActions.GetReachableCells(focusedUnit, focusedUnit.Config.Speed).Keys;

            _grid.StateMachine.Enter<HighlightActionableCellsState, BaseUnit>(focusedUnit);
        }


        private void DehighlightActions()
        {
            _grid.StateMachine.Enter<HighlightActionableCellsState, BaseUnit>(_turnResolver.ActiveUnit);
        }

        private IEnumerable<Cell> GetReachableCellsForAttackStartCell(Cell focusedCell) => 
                _grid
                .GetCellNeighbours(focusedCell)
                .Intersect(_unitActions.ReachableCells.Keys);

        private bool IsEnemyOnNeighbourCell() => 
                _grid
                .GetCellNeighbours(_currentUnit.CurrentCell)
                .Any(cell => _grid.Units.TryGetUnit(cell, out BaseUnit unit) && unit.TeamId != _turnResolver.ActiveUnit.TeamId);

        public void SkipTurn()
        {
            _commandService.SendSkipTurnCommand(_turnResolver.ActiveUnit);
            _localInputStateMachine.Enter<NullCommandState>();
        }
    }
}