using System.Collections.Generic;
using Core.Unit;

namespace Core.Logic.Player
{
    public interface IPlayer
    {
        public List<BaseUnit> ControllableUnits { get; }
        public int TeamId { get; }
        public bool IsLocal { get; }

        public void OnTurn();
    }
}