using System;
using Core.Logic;
using Core.Services;
using Utils.TypeBasedFSM;
using Zenject;

namespace Core.Infrastructure.GameFSM
{
    public class BootstrapState : IState
    {
        private const string InitialSceneName = "Initial";

        private readonly Game _game;
        private readonly IDatabaseService _data;
        private readonly ISceneLoader _sceneLoader;
        private readonly ISoundService _sound;
        private readonly Cursor _cursor;
        private string _mainMenuName;

        [Inject]
        public BootstrapState(Game game, ISceneLoader sceneLoader, IDatabaseService data, Cursor cursor, ISoundService sound)
        {
            _game = game;
            _sceneLoader = sceneLoader;
            _data = data;
            _cursor = cursor;
            _sound = sound;
        }

        public void Enter()
        {
            _data.LoadUnits();
            _data.LoadGameSettings();

            _sound.Init();
            _cursor.Init();
            _mainMenuName = _data.MainMenuScene;

            _sceneLoader.Load(InitialSceneName, onLoaded: EnterLoadMenu);
        }

        public void Exit()
        {
        }

        private void EnterLoadMenu() => _game.StateMachine.Enter<LoadMenuState, string>("MainMenu");
    }
}