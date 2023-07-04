using System;
using Core.Unit.Ability;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class MainMenuWindow : MonoBehaviour
    {
        [SerializeField] private MainMenuButton _newGame;
        [SerializeField] private MainMenuButton _resumeGame;
        [SerializeField] private MainMenuButton _settings;
        [SerializeField] private MainMenuButton _authors;
        [SerializeField] private MainMenuButton _quit;
        [SerializeField] private GameObject _newGameBrief;
        [SerializeField] private Button _newGameBriefNextButton;
        [SerializeField] private Button _authorsCloseButton;
        [SerializeField] private GameObject _authorsWindow;

        public event Action<MainMenuActionId> ButtonClicked;
        public event Action NewGameStarted;
        public event Action ButtonHovered;

        private void Start()
        {
            _newGame.Clicked += OnClick;
            _newGame.Hovered += OnHover;
            // _resumeGame.Clicked += OnClick;
            // _resumeGame.Hovered += OnHover;
            // _settings.Clicked += OnClick;
            // _settings.Hovered += OnHover;
            _authors.Clicked += OnClick;
            _authors.Hovered += OnHover;
            _quit.Clicked += OnClick;
            _quit.Hovered += OnHover;
        }

        private void OnHover()
        {
            ButtonHovered?.Invoke();
        }

        private void OnClick(MainMenuActionId action)
        {
            if (action == MainMenuActionId.NewGame)
            {
                NewGameStarted?.Invoke();
                ShowBrief();
                return;
            }
            else if (action == MainMenuActionId.ShowAuthors)
            {
                ShowAuthors();
                return;
            }
            ButtonClicked?.Invoke(action);
        }

        private void OnBriefSkip()
        {
            _newGameBrief.SetActive(false);
            ButtonClicked?.Invoke(MainMenuActionId.NewGame);
        }

        private void OnAuthorsClose()
        {
            _authorsWindow.SetActive(false);
            ButtonClicked?.Invoke(MainMenuActionId.ShowAuthors);
        }

        private void ShowAuthors()
        {
            _authorsWindow.SetActive(true);
            _authorsCloseButton.onClick.RemoveListener(OnAuthorsClose);
            _authorsCloseButton.onClick.AddListener(OnAuthorsClose);
        }

        private void ShowBrief()
        {
            _newGameBrief.SetActive(true);
            _newGameBriefNextButton.onClick.RemoveListener(OnBriefSkip);
            _newGameBriefNextButton.onClick.AddListener(OnBriefSkip);
        }

        private void OnDestroy()
        {
            _newGame.Clicked -= OnClick;
            _newGame.Hovered -= OnHover;
            // _resumeGame.Clicked -= OnClick;
            // _resumeGame.Hovered -= OnHover;
            // _settings.Clicked -= OnClick;
            // _settings.Hovered -= OnHover;
            _authors.Hovered -= OnHover;
            _authors.Clicked += OnClick;
            _quit.Clicked -= OnClick;
            _quit.Hovered -= OnHover;
        }
    }
}