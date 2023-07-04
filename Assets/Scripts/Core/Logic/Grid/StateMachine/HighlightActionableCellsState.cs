using System.Collections.Generic;
using System.Linq;
using Core.Services;
using Core.Services.Input;
using Core.Unit;
using Utils.TypeBasedFSM;

namespace Core.Logic.Grid.StateMachine
{
    public class HighlightActionableCellsState : IPayloadedState<BaseUnit>
    {
        private readonly IInputService _input;
        private readonly ICellHighlighter _highlighter;
        private readonly IUnitSelector _selector;
        private readonly ITurnResolver _turnResolver;
        private readonly IUnitActionsResolver _unitActions;

        private BaseUnit _selected;

        public HighlightActionableCellsState(IInputService input, ICellHighlighter highlighter, IUnitSelector selector, 
            ITurnResolver turnResolver, IUnitActionsResolver unitActions) 
        {
            _input = input;
            _highlighter = highlighter;
            _selector = selector;
            _turnResolver = turnResolver;
            _unitActions = unitActions;
        }
        
        public void Enter(BaseUnit payload)
        {
            _selected = _turnResolver.ActiveUnit;
            _highlighter.Reset();

            _selected = _turnResolver.ActiveUnit;
            _selector.Select(_selected);
            
            _highlighter.Highlight(GetActionableCells(payload));
        }

        public void Exit() => _highlighter.Reset();

        private IEnumerable<Cell> GetActionableCells(BaseUnit targetUnit) => 
                _unitActions.GetReachableCells(targetUnit, targetUnit.ActionPoints).Keys
                .Union(GetAttackActionableCells(targetUnit)).Distinct();

        private IEnumerable<Cell>  GetAttackActionableCells(BaseUnit targetUnit) =>
                _unitActions.GetAvailableAttacks(targetUnit)
                .Select(attack => attack.target.CurrentCell)
                .Distinct();

    }
}