using System.Collections.Generic;
using System.Linq;
using Core.Database;
using Core.Infrastructure.GameFSM.GameLoop;
using Core.Logic;
using Core.Logic.Grid;
using Core.Logic.Grid.StateMachine;
using Core.Logic.Player;
using Core.Services;
using Core.Services.Input;
using Core.UI;
using Core.UI.HUD;
using Core.Unit;
using Core.Unit.Ability;
using UnityEngine;
using Utils.TypeBasedFSM;
using Zenject;

namespace Core.Infrastructure.GameFSM
{
    public class GameLoopState : IPayloadedState<IEnumerable<IPlayer>, HUD, LevelId>
    {
        private readonly AbstractGrid _grid;
        private readonly Game _game;
        private readonly IGameFactory _gameFactory;
        private readonly ITurnResolver _turnResolver;
        private readonly ICommandInvoker _commandInvoker;
        private readonly ISoundService _sound;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IWindowService _windows;
        private readonly IInputService _input;

        private HUD _hud;
        private GameLoopStateMachine _gameLoopStateMachine;
        private BattleResultResolver _battleResolver;
        private BaseScenario _scenario;
        private List<IPlayer> _players;

        public LevelId CurrentLevel { get; private set; }

        [Inject]
        public GameLoopState(Game game, IGameFactory gameFactory, ICommandInvoker commandInvoker,
            AbstractGrid grid, ITurnResolver turnResolver, ISoundService sound, ICoroutineRunner coroutineRunner,
            IWindowService windows, IInputService input)
        {
            _game = game;
            _gameFactory = gameFactory;
            _turnResolver = turnResolver;
            _commandInvoker = commandInvoker;
            _grid = grid;
            _sound = sound;
            _coroutineRunner = coroutineRunner;
            _windows = windows;
            _input = input;
        }

        public void Enter(IEnumerable<IPlayer> activePlayers, HUD hud, LevelId levelId)
        {
            _gameLoopStateMachine = new GameLoopStateMachine(_gameFactory);
            CurrentLevel = levelId;
            _players = activePlayers.ToList();
            _hud = hud;
            _battleResolver = _gameFactory.CreateBattleResolver(_players);
            _scenario = _gameFactory.CreateScenario(CurrentLevel, _battleResolver, _game.StateMachine);

            _windows.GameLoopButtonClicked += OnUiInput;

            _sound.ThemeState = (float)MusicThemeTypeId.Battle;
            //BattleResolver = new BattleResultResolver(Grid, activePlayers);
            Init();
            _scenario.Init(_players.First(player => player.IsLocal));
            StartGame(_players);

            #region EditorOnlyMode

#if UNITY_EDITOR

            UnityEngine.Object.FindObjectOfType<LoadingCurtain>().Hide();
#endif

            #endregion
        }

        private void OnUiInput(GameLoopButtonAction actionId)
        {
            switch (actionId)
            {
                case GameLoopButtonAction.LoadMenu:
                    _game.StateMachine.Enter<LoadMenuState, string>("MainMenu");
                    break;
            }
        }

        public void Exit()
        {
            _input.DisableRaycaster();
        }

        private void StartGame(IEnumerable<IPlayer> players)
        {
            _turnResolver.Start(players);

            DelegateTurn(to: _turnResolver.CurrentPlayer);
        }

        private void OnUnitCommandSent()
        {
            _grid.StateMachine.Enter<InputLockState>();
            _gameLoopStateMachine.Enter<WaitForTurnState>();
        }

        private void OnUnitCommandExecuted() // TODO rework
        {
            DelegateTurn(to: _turnResolver.CurrentPlayer);
        }

        private void Init()
        {
            _battleResolver.Won += OnGameEnd;

            _sound.ThemeState = (int)MusicThemeTypeId.Battle; // TODO move to another entity
            _commandInvoker.CommandSent += OnUnitCommandSent;
            _commandInvoker.CommandExecuted += OnUnitCommandExecuted;
        }

        private void DelegateTurn(IPlayer to)
        {
            if (to.IsLocal)
            {
                _grid.StateMachine.Enter<HighlightActionableCellsState, BaseUnit>(_turnResolver.ActiveUnit);
                _gameLoopStateMachine.Enter<LocalPlayerInputState, BaseUnit>(_turnResolver.ActiveUnit);
                _hud.AbilitySelected += OnAbilitySelected;
                _hud.AbilityDeselected += OnAbilityDeselected;
                _hud.SkipTurn += OnTurnSkip;
            }
            else
            {
                _grid.StateMachine.Enter<InputLockState>();
                _gameLoopStateMachine.Enter<WaitForTurnState>();
                _hud.AbilitySelected -= OnAbilitySelected;
                _hud.AbilityDeselected -= OnAbilityDeselected;
                _hud.SkipTurn -= OnTurnSkip;
            }
            to.OnTurn();
        }

        private void OnTurnSkip()
        {
            if (_gameLoopStateMachine.CurrentState is LocalPlayerInputState state)
            {
                state.SkipTurn();
                _hud.SkipTurn -= OnTurnSkip;
                Debug.Log($"{_turnResolver.ActiveUnit} SKIP GAMELOOP");
            }
        }

        private void OnAbilitySelected(BaseAbility selected)
        {
            if (_gameLoopStateMachine.CurrentState is LocalPlayerInputState state) 
                state.OnAbilitySelected(selected);
        }

        private void OnAbilityDeselected()
        {
            if (_gameLoopStateMachine.CurrentState is LocalPlayerInputState state)
                state.OnAbilityDeselected();
        }

        private void OnGameEnd(int winnerId)
        {
            if (winnerId == _players.First(player => player.IsLocal).TeamId)
                _sound.ThemeState = (float)MusicThemeTypeId.Win;
            else
                _sound.ThemeState = (float)MusicThemeTypeId.Lose;

        }
    }
}