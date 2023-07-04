using Core.Infrastructure.GameFSM;
using Utils.TypeBasedFSM;
using Zenject;

namespace Core.Infrastructure.Factories
{
    public class SystemFactory : ISystemFactory
    {
        private readonly DiContainer _diContainer;

        public SystemFactory(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public Game CreateGame() => _diContainer.Instantiate<Game>();

        public GameStateMachine CreateGameStateMachine(Game game)
        {
            return _diContainer.Instantiate<GameStateMachine>(new[] { game });
        }

        public TState CreateState<TState>(Game game) where TState : class, IExitableState => 
            _diContainer.Instantiate<TState>(new[] { game });

        public IStateDefinition<TState> DefineState<TState>() where TState : class, IExitableState => 
            new StateDefinition<TState>(this);

        public TState CreateState<TState>() where TState : class, IExitableState => 
            _diContainer.Instantiate<TState>();
        
        public TState CreateState<TState>(IGameStateMachine stateMachine) where TState : class, IExitableState => 
            _diContainer.Instantiate<TState>(new []{ stateMachine});
    }
}