using Utils.TypeBasedFSM;
using Zenject;

namespace Core.Infrastructure.GameFSM
{
    public class StateDefinition<TState> : IStateDefinition<TState> where TState : class, IExitableState 
    {
        private readonly IStateFactory _stateFactory;

        public StateDefinition(IStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }

        public TState GetState() => _stateFactory.CreateState<TState>();
    }
}