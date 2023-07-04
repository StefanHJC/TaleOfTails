using System;
using Core.Infrastructure.GameFSM;
using Core.Logic;
using UnityEngine;
using Zenject;

namespace Core.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private LoadingCurtain _curtain;

        private ISystemFactory _systemFactory;
        private Game _game;

        public event Action SceneChanged;

        [Inject]
        public void Construct(ISystemFactory systemFactory)
        {
            _systemFactory = systemFactory;
        }

        private void Awake()
        {
            _game = _systemFactory.CreateGame();
            _game.StateMachine.Enter<BootstrapState>();

            DontDestroyOnLoad(this);
        }

        private void OnLevelWasLoaded()
        {
             _game.Reinit();
             GC.Collect();
        }
    }
}