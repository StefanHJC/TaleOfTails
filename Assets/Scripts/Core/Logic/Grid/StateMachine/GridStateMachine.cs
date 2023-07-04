using System;
using System.Collections.Generic;
using Core.Services;
using Core.Services.Input;
using Utils.TypeBasedFSM;

namespace Core.Logic.Grid.StateMachine
{
    public class GridStateMachine : TypeBasedStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states;

        public override IReadOnlyDictionary<Type, IExitableState> States => _states;

        public GridStateMachine(IInputService input, ICellHighlighter highlighter, IUnitSelector selector,
            UnitsOnGrid units, IPathRenderer pathRenderer, ITurnResolver turnResolver, IUnitActionsResolver unitActions)
        {
            _states = new Dictionary<Type, IExitableState>
            {
                [typeof(HighlightActionableCellsState)] = new HighlightActionableCellsState(input, highlighter, selector, turnResolver, unitActions),
                [typeof(Highlight—oncreteCellsState)] = new Highlight—oncreteCellsState(highlighter),
                [typeof(InputLockState)] = new InputLockState(pathRenderer, highlighter, selector),
            };
        }
    }
}