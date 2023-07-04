using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Database;
using Core.Logic;
using Core.Logic.Grid;
using Core.Logic.Player;
using Core.Services;
using Core.Services.Input;
using Core.UI.HUD;
using Core.Unit;
using UnityEngine;
using Utils;
using Utils.TypeBasedFSM;
using Zenject;

namespace Core.Infrastructure.GameFSM
{
    public class LoadLevelState : IPayloadedState<LevelId>
    {
        private readonly LoadingCurtain _curtain;
        private readonly ISceneLoader _sceneLoader;
        private readonly IInputService _input;
        private readonly IUnitFactory _unitFactory;
        private readonly IGameFactory _gameFactory;
        private readonly ISoundService _sound;
        private readonly IDatabaseService _data;
        private readonly Game _game;

        private LevelId _currentLevelId;
        private ISystemFactory _systemFactory;

        [Inject]
        public LoadLevelState(Game game, ISceneLoader sceneLoader, LoadingCurtain curtain,
            IInputService input, IUnitFactory unitFactory, IGameFactory gameFactory, ISoundService sound,
            IDatabaseService data, ISystemFactory systemFactory)
        {
            _game = game;
            _sceneLoader = sceneLoader;
            _curtain = curtain;
            _input = input;
            _unitFactory = unitFactory;
            _gameFactory = gameFactory;
            _sound = sound;
            _data = data;
            _systemFactory = systemFactory;
        }

        public void Enter(LevelId id)
        {
            _currentLevelId = id;
            _curtain?.Show();
            LevelData level = _data.TryGetLevelData(_currentLevelId);

            _sound.ThemeState = (float)MusicThemeTypeId.Menu;
            _sceneLoader.Load(level.SceneName, onLoaded: InitGame);
        }

        public void Exit()
        {
            _curtain?.Hide();
        }

        private IEnumerator SwitchMusicRoutine() // TODO rework
        {
            while (_sceneLoader.Status == SceneLoader.OperationStatus.Progress)
            {
                _sound.ThemeState = Mathf.MoveTowards((float)_sound.ThemeState, (int)MusicThemeTypeId.Battle, _sceneLoader.Progress);
                yield return null;
            }
        }

        private void OnInitialSceneLoaded()
        {
            LevelData level = _data.TryGetLevelData(_currentLevelId);
            _sceneLoader.Load(level.SceneName, onLoaded: InitGame);
        }

        private void InitGame()
        {
            SceneContext sceneContext = GetSceneContext();

            _input.Init();
            _gameFactory.Init(sceneContext);
            _unitFactory.Init(sceneContext);
            
            if (_game.StateMachine._states.ContainsKey(typeof(GameLoopState)) == false)
                _game.StateMachine._states.Add(typeof(GameLoopState), _gameFactory.CreateState<GameLoopState>(_game));

            _gameFactory.UI.CreateUiRoot();
            List<BaseUnit> spawnedUnits = SpawnUnits(sceneContext.Container.Resolve<AbstractGrid>());
            IEnumerable<IPlayer> players = CreatePlayers(spawnedUnits);
            //_curtain.Hide();
            Object.FindObjectOfType<LoadingCurtain>().gameObject.SetActive(false);
            _game.StateMachine.Enter<GameLoopState, IEnumerable<IPlayer>, HUD, LevelId>(players, _gameFactory.UI.CreateHUD(), _currentLevelId);
        }

        private List<IPlayer> CreatePlayers(IEnumerable<BaseUnit> spawnedUnits) // TODO rework
        {
            ILookup<int, BaseUnit> unitsByTeams = GetUnitsByTeams(spawnedUnits);

            List<IPlayer> players = new List<IPlayer>();
            players.Add(_gameFactory.CreatePlayer<LocalPlayer>(0, unitsByTeams[0].ToList()));
            players.Add(_gameFactory.CreatePlayer<AIPlayer>(1, unitsByTeams[1].ToList()));

            return players;
        }

        private ILookup<int, BaseUnit> GetUnitsByTeams(IEnumerable<BaseUnit> units) => units.ToLookup(unit => unit.TeamId);

        private List<BaseUnit> SpawnUnits(AbstractGrid grid)
        {
            UnitSpawnTag[] unitInitialSpawnTags = grid.GetComponentsInChildren<UnitSpawnTag>();
            List<BaseUnit> spawnedUnits = new List<BaseUnit>();

            for (int i = 0; i < unitInitialSpawnTags.Length; i++)
            {
                var rotation = new Quaternion();
                if (grid.GridCenter != null)
                {
                    rotation = Quaternion.LookRotation(grid.GridCenter.transform.position -
                                                       unitInitialSpawnTags[i].GetComponent<Cell>().transform.position);
                }

                var unit = _unitFactory.CreateUnit(unitInitialSpawnTags[i].GetComponent<Cell>(),
                    unitInitialSpawnTags[i].UnitType, unitInitialSpawnTags[i].TeamId, rotation);
                unit.CurrentCell = unitInitialSpawnTags[i].GetComponent<Cell>();
                spawnedUnits.Add(unit);
            }
            return spawnedUnits;
        }

        private SceneContext GetSceneContext() => Object.FindObjectOfType<SceneContext>();
    }
}