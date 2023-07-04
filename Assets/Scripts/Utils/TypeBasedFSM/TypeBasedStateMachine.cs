using System;
using System.Collections.Generic;
using Core.Infrastructure.GameFSM;

namespace Utils.TypeBasedFSM
{
    public abstract class TypeBasedStateMachine : IGameStateMachine
    {
        private IExitableState _currentState;

        public IExitableState CurrentState => _currentState;
        public abstract IReadOnlyDictionary<Type, IExitableState> States { get; }

        public virtual void Enter<TState>() where TState : class, IState
        {
            TState state = SwitchState<TState>();
            state.Enter();
        }

        public virtual void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            _currentState?.Exit();
            IPayloadedState<TPayload> state = GetState<TState>();
            _currentState = state;
            state.Enter(payload);
        }
        
        public virtual void Enter<TState, TPayload1, TPayload2>(TPayload1 payload1, TPayload2 payload2) where TState : class, IPayloadedState<TPayload1, TPayload2>
        {
            _currentState?.Exit();
            IPayloadedState<TPayload1, TPayload2> state = GetState<TState>();
            _currentState = state;
            state.Enter(payload1, payload2);
        }
        public virtual void Enter<TState, TPayload1, TPayload2, TPayload3>(TPayload1 payload1, TPayload2 payload2, TPayload3 payload3) where TState : class, IPayloadedState<TPayload1, TPayload2, TPayload3>
        {
            _currentState?.Exit();
            IPayloadedState<TPayload1, TPayload2, TPayload3> state = GetState<TState>();
            _currentState = state;
            state.Enter(payload1, payload2, payload3);
        }

        protected virtual TState SwitchState<TState>() where TState : class, IExitableState
        {
            _currentState?.Exit();
            TState state = GetState<TState>();
            _currentState = state;

            return state;
        }

        protected virtual TState GetState<TState>() where TState : class, IExitableState => States[typeof(TState)] as TState;
    }
}