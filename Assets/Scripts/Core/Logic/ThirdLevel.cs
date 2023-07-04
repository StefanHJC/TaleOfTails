using System.Collections;
using Core.Database;
using Core.Infrastructure.GameFSM;
using Core.UI;
using UnityEngine;

namespace Core.Logic
{
    public class ThirdLevel : BaseScenario
    {
        protected override void OnLevelStart()
        {
            StartCoroutine(DialogueDelay());
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
                    GameStateMachine.Enter<LoadMenuState, string>("MainMenu");
                    break;
                }
                case GameLoopButtonAction.RetryLevel:
                {
                    GameStateMachine.Enter<LoadMenuState, string>("MainMenu");
                    break;
                }
            }
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
            yield return new WaitForSeconds(1f);

            Dialogue.PlayDialogue(StartLevelDialogue);
        }
    }
}