using System.Collections.Generic;
using Core.Database;
using Core.Infrastructure.GameFSM;
using Core.Logic;
using Core.Logic.AI;
using Core.Logic.Player;
using Core.Services;
using Core.Unit;
using Utils.TypeBasedFSM;
using Zenject;

namespace Core.Infrastructure.Factories
{
    public class GameFactory : IGameFactory
    {
        private readonly UIFactory _uiFactory;

        private DiContainer _diContainer;
        private readonly IDatabaseService _data;

        public UIFactory UI => _uiFactory;

        [Inject]
        public GameFactory(DiContainer diContainer, IDatabaseService data)
        {
            _diContainer = diContainer;
            _uiFactory = _diContainer.Instantiate<UIFactory>();
            _data  = data;
        }

        public void Init(SceneContext sceneContext)
        {
            _diContainer = sceneContext.Container;
            _uiFactory.Init(_diContainer);
        }

        public TPlayer CreatePlayer<TPlayer>(int teamId, List<BaseUnit> units) where TPlayer : IPlayer => 
            _diContainer.Instantiate<TPlayer>(new object[] { teamId, units});

        public IArtificialIntelligence CreateAI() => _diContainer.Instantiate<AI>();

        public IStateDefinition<TState> DefineState<TState>() where TState : class, IExitableState => 
            new StateDefinition<TState>(this);

        public TState CreateState<TState>() where TState : class, IExitableState => 
            _diContainer.Instantiate<TState>();

        public TState CreateState<TState>(IGameStateMachine stateMachine) where TState : class, IExitableState => 
            _diContainer.Instantiate<TState>(new[] { stateMachine });

        public TState CreateState<TState>(Game game) where TState : class, IExitableState => 
            _diContainer.Instantiate<TState>(new []{game});

        public BattleResultResolver CreateBattleResolver(IEnumerable<IPlayer> players) => 
            _diContainer.Instantiate<BattleResultResolver>(new[] {players});

        public BaseScenario CreateScenario(LevelId level, BattleResultResolver resolver, GameStateMachine stateMachine)
        {
            BaseScenario levelScenario = _data.TryGetLevelData(level).ScenarioScript;
            BaseScenario scenarioInstance = UnityEngine.Object.Instantiate(levelScenario);
            _diContainer.Inject(scenarioInstance, new object[] {resolver, stateMachine});

            return scenarioInstance;
        }
    }
}