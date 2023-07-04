using System.Collections.Generic;
using System.Linq;
using Core.Database;
using Core.Infrastructure;
using Core.Infrastructure.GameFSM;
using Core.Logic.Grid;
using Core.Logic.Player;
using Core.Services;
using Core.Services.Input;
using Core.UI;
using Core.UI.HUD;
using Core.Unit;
using FMOD;
using UnityEngine;
using Zenject;

namespace Core.Logic
{
    public abstract class BaseScenario : MonoBehaviour
    {
        [SerializeField] protected LevelId LevelId;
        [SerializeField] protected TextAsset StartLevelDialogue;
        [SerializeField] protected TextAsset EndLevelDialogue;

        protected BattleResultResolver BattleResolver;
        protected AbstractGrid Grid;
        protected IPlayer LocalPlayer;
        protected IWindowService WindowService;
        protected GameStateMachine GameStateMachine;
        protected DialoguePlayer Dialogue;
        protected HUD Hud;
        protected IInputService InputService;

        [Inject]
        public void Construct(BattleResultResolver battleResult, AbstractGrid grid, IWindowService windowService,
            GameStateMachine gameStateMachine, DialoguePlayer dialogue, IGameFactory gameFactory, IInputService input, ISoundService sound)
        {
            BattleResolver = battleResult;
            Grid = grid;
            Dialogue = dialogue;
            Hud = gameFactory.UI.HUD;
            WindowService = windowService;
            InputService = input;
            WindowService.GameLoopButtonClicked += OnUiAction;
            GameStateMachine = gameStateMachine;
            SoundService = sound;
        }

        public ISoundService SoundService { get; set; }

        public void Init(IPlayer localPlayer)
        {
            Grid.Units.UnitDied += OnUnitDeath;
            BattleResolver.Won += OnGameEnd;
            LocalPlayer = localPlayer;
        }

        private void Start()
        {
            OnLevelStart();
        }

        private void OnDestroy()
        {
            Grid.Units.UnitDied -= OnUnitDeath;
            BattleResolver.Won -= OnGameEnd;
        }

        protected virtual void OnLevelStart()
        {
        }

        protected virtual void OnUiAction(GameLoopButtonAction action)
        {
            switch (action)
            {
                case GameLoopButtonAction.NextLevel:
                {
                    GameStateMachine.Enter<LoadLevelState, LevelId>(++LevelId);
                    break;
                }
                case GameLoopButtonAction.RetryLevel:
                {
                    GameStateMachine.Enter<LoadLevelState, LevelId>(LevelId);
                    break;
                }
            }
        }

        protected virtual void OnGameEnd(int winnerTeamId)
        {
        }

        protected void SetCatsDance()
        {
            IEnumerable<BaseUnit> cats = FindObjectsOfType<BaseUnit>()
                .Where(unit => unit.TeamId == LocalPlayer.TeamId);

            foreach (BaseUnit cat in cats)
                cat.GetComponent<BaseUnitAnimator>().PlayDance();
        }

        protected void OnUnitDeath(BaseUnit obj)
        {
        }
    }
}