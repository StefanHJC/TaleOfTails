using System;
using System.Collections.Generic;
using System.Linq;
using Core.Infrastructure.GameFSM;
using Core.Logic.Grid;
using Core.Logic.Player;
using Core.Services;
using Core.UI;
using Core.Unit;

namespace Core.Logic
{
    public class BattleResultResolver
    {
        private readonly AbstractGrid _grid;
        private readonly IEnumerable<IPlayer> _players;
        private readonly IWindowService _windowService;
        private readonly GameStateMachine _gameStateMachine;
        private readonly ISoundService _sound;

        public event Action<int> Won;

        public BattleResultResolver(AbstractGrid grid, IEnumerable<IPlayer> players, IWindowService windowService, ISoundService sound)
        {
            _grid = grid;
            _windowService = windowService;
            _grid.Units.UnitDied += OnUnitDeath;
            _players = players;
            _sound = sound;
        }

        public void OnGameWin()
        {
            _sound.ThemeState = (float)MusicThemeTypeId.Win;
            _windowService.Open(WindowId.WinWindow);
        }

        public void OnGameLost()
        {
            _sound.ThemeState = (float)MusicThemeTypeId.Lose;
            _windowService.Open(WindowId.LoseWindow);
        }

        private void OnUnitDeath(BaseUnit unit)
        {
            IPlayer unitOwner = _players.First(player => player.TeamId == unit.TeamId);
            IPlayer secondPlayer = _players.First(player => player.TeamId != unit.TeamId);

            unitOwner.ControllableUnits.Remove(unit);

            if (unitOwner.ControllableUnits.Count <= 0)
            {
                Won?.Invoke(secondPlayer.TeamId);
            }
        }
    }
}