using System.Collections.Generic;
using Core.Infrastructure;
using Core.Logic.AI;
using Core.Unit;
using Zenject;

namespace Core.Logic.Player
{
    public class AIPlayer : IPlayer
    {
        private List<BaseUnit> _units;
        private int _teamdId;
        private IArtificialIntelligence _ai;

        public List<BaseUnit> ControllableUnits => _units;
        public int TeamId => _teamdId;
        public bool IsLocal => false;

        [Inject]
        public AIPlayer(int teamdId, List<BaseUnit> units, IGameFactory gameFactory)
        {
            _teamdId = teamdId;
            _units = units;
            _ai = gameFactory.CreateAI();
        }

        public void OnTurn() => _ai.Awake();
    }
}