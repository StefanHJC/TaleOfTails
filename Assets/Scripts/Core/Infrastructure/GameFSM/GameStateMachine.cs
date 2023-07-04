using System;
using System.Collections.Generic;
using Utils.TypeBasedFSM;
using Zenject;

namespace Core.Infrastructure.GameFSM
{
    public class GameStateMachine : TypeBasedStateMachine
    {
        public Dictionary<Type, IExitableState> _states;

        public override IReadOnlyDictionary<Type, IExitableState> States => _states;

        [Inject]
        public GameStateMachine(Game game, ISystemFactory systemFactory)
        {
            _states = new Dictionary<Type, IExitableState>
            {
                [typeof(BootstrapState)] = systemFactory.CreateState<BootstrapState>(game),
                [typeof(LoadMenuState)] = systemFactory.CreateState<LoadMenuState>(game),
                [typeof(MainMenuState)] = systemFactory.CreateState<MainMenuState>(game),
                [typeof(LoadLevelState)] = systemFactory.CreateState<LoadLevelState>(game),
            };
        }
    }
}