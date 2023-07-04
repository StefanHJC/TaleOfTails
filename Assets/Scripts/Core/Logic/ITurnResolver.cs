using System;
using System.Collections.Generic;
using Core.Logic.Player;
using Core.Unit;

namespace Core.Logic
{
    public interface ITurnResolver
    {
        public BaseUnit ActiveUnit { get; }
        public IPlayer CurrentPlayer { get; }
        public IReadOnlyList<BaseUnit> UnitTurnOrder { get; }
        public IReadOnlyList<BaseUnit> Units { get; }
        public int CurrentRound { get; }
        
        public event Action TurnStarted;
        public event Action RoundStarted;

        public void Start(IEnumerable<IPlayer> players);
        public IReadOnlyList<BaseUnit> GetTurnOrderForNextRound();
    }
}