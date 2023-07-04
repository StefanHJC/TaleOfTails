using System;
using System.Collections.Generic;
using Utils.TypeBasedFSM;

namespace Core.Infrastructure.GameFSM.GameLoop
{
    public class GameLoopStateMachine : TypeBasedStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states;

        public override IReadOnlyDictionary<Type, IExitableState> States => _states;

        public GameLoopStateMachine(IGameFactory gameFactory)
        {
            _states = new Dictionary<Type, IExitableState>
            {
                [typeof(LocalPlayerInputState)] = gameFactory.CreateState<LocalPlayerInputState>(),
                [typeof(GameInterruptState)] = gameFactory.CreateState<GameInterruptState>(),
                [typeof(WaitForTurnState)] = gameFactory.CreateState<WaitForTurnState>(),
            };
        }
    }
}