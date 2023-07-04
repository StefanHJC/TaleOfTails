using System;
using Core.Infrastructure.Factories;
using Core.Infrastructure.GameFSM;

namespace Core.Infrastructure
{
    public class Game
    {
        private readonly ISystemFactory _systemFactory;
        public GameStateMachine StateMachine { get; private set; }

        public Game(ISystemFactory systemFactory)
        {
            _systemFactory = systemFactory;

            StateMachine = new GameStateMachine(this, systemFactory);
            var aa = StateMachine;
        }

        public void Reinit()
        {
            StateMachine = new GameStateMachine(this, _systemFactory);
        }
    }
}
