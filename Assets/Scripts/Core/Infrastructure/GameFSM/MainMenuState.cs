using System.Collections;
using Core.Database;
using Core.Logic;
using Core.Services;
using Core.UI;
using FMODUnity;
using UnityEngine;
using Utils.TypeBasedFSM;

namespace Core.Infrastructure.GameFSM
{
    public class MainMenuState : IState
    {
        private readonly Game _game;
        private readonly ISceneLoader _sceneLoader;
        private readonly IDatabaseService _data;
        private readonly ISoundService _sound;
        private readonly EventReference _buttonHoverSfx;
        private readonly EventReference _buttonClickSfx;
        private readonly EventReference _briefSkipSfx;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ISystemFactory _systemFactory;

        private MainMenuWindow _mainMenu;
        private bool _isLoading;
        private bool _isNewGame;

        public MainMenuState(Game game, ISceneLoader sceneLoader, IDatabaseService data,
            ISoundService sound, LoadingCurtain loadingCurtain, ICoroutineRunner coroutineRunner, ISystemFactory systemFactory)
        {
            _game = game;
            _sceneLoader = sceneLoader;
            _data = data;
            _sound = sound;
            _loadingCurtain = loadingCurtain;
            _coroutineRunner = coroutineRunner;
            _systemFactory = systemFactory;
            MusicSettings musicData = _data.TryGetMusicData();

            _buttonHoverSfx = musicData.MainMenuButtonHovered;
            _buttonClickSfx = musicData.MainMenuButtonClicked;
            _briefSkipSfx = musicData.MenuWindowClosed;
        }

        public void Enter()
        {
            _sound.ThemeState = (float)MusicThemeTypeId.Menu;
            _mainMenu = Object.FindObjectOfType<MainMenuWindow>();
            _mainMenu.ButtonClicked += OnMenuButtonClick;
            _mainMenu.ButtonHovered += OnMenuButtonHovered;
            _mainMenu.NewGameStarted -= OnNewGameStart;
            _isLoading = false;
        }

        private void OnNewGameStart()
        {
            _isNewGame = true;
        }

        public void Exit()
        {
            _mainMenu.NewGameStarted -= OnNewGameStart;
            _mainMenu.ButtonClicked -= OnMenuButtonClick;
            _mainMenu.ButtonHovered -= OnMenuButtonHovered;
            _loadingCurtain.Show();
            _sound.ThemeState = (float)MusicThemeTypeId.Battle;
        }

        private void OnMenuButtonClick(MainMenuActionId action)
        {
            if (_isLoading)
                return;
            
            _sound.PlayOnce(_buttonClickSfx);
            switch (action)
            {
                case MainMenuActionId.NewGame:
                    PlayLevel(LevelId.First);
                    break;
                case MainMenuActionId.ResumeGame:
                    break;
                case MainMenuActionId.ShowAuthors:
                    break;
                case MainMenuActionId.ShowSettings:
                    break;
                case MainMenuActionId.Quit:
                    Application.Quit();
                    break;
                case MainMenuActionId.None:
                    break;
            }
        }

        private void OnMenuButtonHovered() => _sound.PlayOnce(_buttonHoverSfx);

        private void PlayLevel(LevelId levelId)
        {
            if (_game.StateMachine._states.ContainsKey(typeof(LoadLevelState)) == false)
                _game.StateMachine._states.Add(typeof(LoadLevelState), _systemFactory.CreateState<LoadLevelState>(_game));
            
            _isLoading = true;
            _coroutineRunner.StartCoroutine(StartGame(levelId));
        }

        private void StartNewGame()
        {
            _isLoading = true;
            _coroutineRunner.StartCoroutine(StartGame(LevelId.First));
        }

        private IEnumerator StartGame(LevelId levelId)
        {
            _loadingCurtain.ShowWithFade();
            yield return new WaitForSeconds(1.5f);


            LevelData data = _data.TryGetLevelData(levelId);
            _game.StateMachine.Enter<LoadLevelState, LevelId>(data.LevelId);
        }
    }
}