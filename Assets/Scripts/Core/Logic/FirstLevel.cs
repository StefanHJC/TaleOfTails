using System.Collections;
using Core.Database;
using Core.Infrastructure.GameFSM;
using Core.UI;
using UnityEngine;

namespace Core.Logic
{
    public class FirstLevel : BaseScenario
    {
        protected override void OnLevelStart()
        {
            Hud.gameObject.SetActive(false);
            InputService.DisableRaycaster();

            BaseWindow window = WindowService.Open(WindowId.EducationWindow);
            window.Closed += EnableInput;
        }

        protected override void OnGameEnd(int winnerTeamId)
        {
            if (winnerTeamId != LocalPlayer.TeamId)
            {
                BattleResolver.OnGameLost();
                return;
            }

            Dialogue.PlayDialogue(EndLevelDialogue);
            Dialogue.DialogueEnded += ShowWinWindow;
        }

        protected override void OnUiAction(GameLoopButtonAction action)
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
                    GameStateMachine.Enter<LoadMenuState, string>("MainMenu");
                    break;
                }
            }
        }

        private void EnableInput()
        {
            Hud.gameObject.SetActive(true);
            InputService.EnableRaycaster();
        }

        private void ShowWinWindow()
        {
            BattleResolver.OnGameWin();
            SetCatsDance();

            Dialogue.DialogueEnded -= ShowWinWindow;
        }

        private IEnumerator DialogueDelay()
        {
            SoundService.ThemeState = (float)MusicThemeTypeId.Dialogue;
            yield return new WaitForSeconds(2f);
            Dialogue.PlayDialogue(StartLevelDialogue);
        }
    }
}