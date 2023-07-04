using System;
using Core.Infrastructure;
using Core.UI;
using Zenject;

namespace Core.Services
{
    public class WindowService : IWindowService
    {
        private readonly ISoundService _sound;
        private readonly IGameFactory _gameFactory;

        public event Action<GameLoopButtonAction> GameLoopButtonClicked;

        [Inject]
        public WindowService(ISoundService sound, IGameFactory gameFactory)
        {
            _sound = sound;
            _gameFactory = gameFactory;
        }

        public BaseWindow Open(WindowId windowId)
        {
            switch (windowId)
            {
                case WindowId.None:
                    break;

                case WindowId.SystemMenu:
                    var systemScreen = _gameFactory.UI.CreateSystemMenu();
                    systemScreen.ButtonClicked += OnUiEvent;

                    return systemScreen;

                case WindowId.WinWindow:
                    var winScreen = _gameFactory.UI.CreateWinScreen();
                    winScreen.ButtonClicked += OnUiEvent;

                    return winScreen;

                case WindowId.LoseWindow:
                    var loseScreen = _gameFactory.UI.CreateLoseScreen();
                    loseScreen.ButtonClicked += OnUiEvent;

                    return loseScreen;

                case WindowId.DialogueWindow:
                    var diaWindow = _gameFactory.UI.CreateDialogueWindow();

                    return diaWindow;

                case WindowId.EducationWindow:
                    var educationalWindow = _gameFactory.UI.CreateEducationalWindow();

                    return educationalWindow;

                case WindowId.Authors:
                    var authorsWindow = _gameFactory.UI.CreateAuthorsWindow();

                    return authorsWindow;
            }
            return null;
        }

        private void OnUiEvent(GameLoopButtonAction actionId, BaseWindow invoker)
        {
            invoker.ButtonClicked -= OnUiEvent;
            GameLoopButtonClicked?.Invoke(actionId);
        }
    }
}