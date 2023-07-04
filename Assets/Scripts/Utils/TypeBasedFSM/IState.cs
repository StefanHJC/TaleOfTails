using System;
using ModestTree.Util;

namespace Utils.TypeBasedFSM
{
    public interface IState : IExitableState
    {
        public void Enter();
    }

    public interface IExitableState
    {
        public void Exit();
    }
    
    public interface IPayloadedState<TPayload> : IExitableState
    {
        public void Enter(TPayload payload);
    }
    
    public interface IPayloadedState<TPayload1, TPayload2> : IExitableState
    {
        public void Enter(TPayload1 payload1, TPayload2 payload2);
    }   
    
    public interface IPayloadedState<TPayload1, TPayload2, TPayload3> : IExitableState
    {
        public void Enter(TPayload1 payload1, TPayload2 payload2, TPayload3 payload3);
    }

    public interface IParameterState<TParameter> : IExitableState
    {
        public TParameter Parameter { get; }
    }
}