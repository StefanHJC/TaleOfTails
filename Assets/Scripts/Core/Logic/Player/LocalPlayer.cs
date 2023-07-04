using System.Collections.Generic;
using Core.Unit;

namespace Core.Logic.Player
{
    public class LocalPlayer : IPlayer
    {
        private List<BaseUnit> _units; 
        private int _teamId;

        public List<BaseUnit> ControllableUnits => _units;
        public int TeamId => _teamId;
        public bool IsLocal => true;

        public LocalPlayer(int teamId, List<BaseUnit> units)
        {
            _teamId = teamId;
            _units = units;
        }

        public void OnTurn()
        {
        }
    }
}