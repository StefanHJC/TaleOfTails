using System;
using System.Collections;
using Core.Logic;
using Core.Services;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.TypeBasedFSM;
using Zenject;

namespace Core.Infrastructure.GameFSM
{
    public class LoadMenuState : IPayloadedState<string>
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly Game _game;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ISoundService _sound;

        [Inject]
        public LoadMenuState(Game game, ISceneLoader sceneLoader, LoadingCurtain loadingCurtain, ICoroutineRunner coroutineRunner, 
            ISoundService sound)
        {
            _sceneLoader = sceneLoader;
            _game = game;
            _loadingCurtain = loadingCurtain;
            _coroutineRunner = coroutineRunner;
            _sound = sound;
        }

        public void Enter(string payload)
        {
            _sound.ThemeState = (float)MusicThemeTypeId.Menu;
            SceneManager.LoadScene(payload);
            
            SceneManager.activeSceneChanged += OnSceneLoaded;
            _sound.ThemeState = (float)MusicThemeTypeId.Menu;
        }

        private void OnSceneLoaded(Scene arg0, Scene arg1)
        {
            if (arg1.name != "MainMenu")
                return;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));
            _game.StateMachine.Enter<MainMenuState>();
            _loadingCurtain.Hide();
        }


        public void Exit()
        {
            SceneManager.activeSceneChanged -= OnSceneLoaded;
        }
    }
}