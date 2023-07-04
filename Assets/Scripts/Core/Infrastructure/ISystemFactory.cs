using System.Collections.Generic;
using Core.Database;
using Core.Infrastructure.Factories;
using Core.Infrastructure.GameFSM;
using Core.Logic;
using Core.Logic.AI;
using Core.Logic.Player;
using Core.UI.HUD;
using Core.Unit;
using UnityEngine;
using Utils;
using Utils.TypeBasedFSM;
using Zenject;

namespace Core.Infrastructure
{
    public interface ISystemFactory : IStateFactory
    {
        public Game CreateGame();
        public GameStateMachine CreateGameStateMachine(Game game);
    }

    public interface IStateFactory
    {
        public IStateDefinition<TState> DefineState<TState>() where TState : class, IExitableState;
        public TState CreateState<TState>() where TState : class, IExitableState;
        public TState CreateState<TState>(IGameStateMachine stateMachine) where TState : class, IExitableState;
        public TState CreateState<TState>(Game game) where TState : class, IExitableState;
    }

    public interface IGameFactory : IStateFactory
    {
        public UIFactory UI { get; }

        public void Init(SceneContext sceneContext);
        public TPlayer CreatePlayer<TPlayer>(int teamId, List<BaseUnit> units) where TPlayer : IPlayer;
        public IArtificialIntelligence CreateAI();
        public BattleResultResolver CreateBattleResolver(IEnumerable<IPlayer> players);
        public BaseScenario CreateScenario(LevelId level, BattleResultResolver resolver, GameStateMachine stateMachine);
    }

    public interface IUIFactory
    {
        public Canvas Root { get; }

        public void Init(DiContainer diContainer);
        public HUD CreateHUD();
        public Canvas CreateUiRoot();
        public HealthRenderer CreateHealthRenderer();
    }
}