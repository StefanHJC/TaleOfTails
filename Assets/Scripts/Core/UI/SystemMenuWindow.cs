using System.Collections;
using System.Collections.Generic;
using Core.Database;
using Core.Infrastructure;
using Core.Logic;
using Core.Services;
using Core.Services.Input;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.UI
{
    public class SystemMenuWindow : BaseWindow
    {
        [SerializeField] private List<ButtonBehaviour> _buttons;

        [SerializeField] private Button _enterMenu;

        private IInputService _input;
        private HUD.HUD _hud;
        private ISoundService _sound;
        private EventReference _buttonHoverSfx;
        private EventReference _buttonClickSfx;
        private LoadingCurtain _curtain;

        [Inject]
        public void Construct(IInputService input, IGameFactory gameFactory, ISoundService sound, IDatabaseService data, LoadingCurtain curtain)
        {
            _hud = gameFactory.UI.HUD;
            _input = input;
            _hud.gameObject.SetActive(false);
            _input.DisableRaycaster();
            _sound = sound;
            _curtain = curtain;
            MusicSettings musicData = data.TryGetMusicData();

            _buttonHoverSfx = musicData.MainMenuButtonHovered;
            _buttonClickSfx = musicData.MenuWindowOpened;
            _enterMenu.onClick.AddListener(OnMenuButtonClick);

            foreach (ButtonBehaviour button in _buttons)
            {
                button.Clicked += OnButtonClicked;
                button.Hovered += OnButtonHovered;
            }
        }

        private void OnMenuButtonClick()
        {
            _sound.PlayOnce(_buttonClickSfx);
            StartCoroutine(ExitDelay());
        }

        public IEnumerator ExitDelay()
        {
            _curtain.ShowWithFade();
            yield return new WaitForSeconds(1.5f);
            ButtonClicked?.Invoke(GameLoopButtonAction.LoadMenu, this);
        }

        private void OnButtonHovered() => _sound.PlayOnce(_buttonHoverSfx);

        private void OnButtonClicked() => _sound.PlayOnce(_buttonClickSfx);

        private void OnDestroy()
        {
            _input.EnableRaycaster();
            _hud.gameObject.SetActive(true);
            _enterMenu.onClick.AddListener(OnMenuButtonClick);

            foreach (ButtonBehaviour button in _buttons)
            {
                button.Clicked -= OnButtonClicked;
                button.Hovered -= OnButtonHovered;
            }
        }
    }
}