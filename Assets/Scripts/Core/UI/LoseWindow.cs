using System.Collections;
using Core.Infrastructure;
using Core.Logic;
using Core.Services.Input;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.UI
{
    public class LoseWindow : BaseWindow
    {
        [SerializeField] private Button _goNextButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        private HUD.HUD _hud;
        private IInputService _input;

        [Inject]
        public void Construct(IGameFactory gameFactory, IInputService input)
        {
            _hud = gameFactory.UI.HUD;
            _input = input;
            _hud.gameObject.SetActive(false);
            _input.DisableRaycaster();
        }

        private void Awake()
        {
            _canvasGroup.alpha = 0;
            _goNextButton.onClick.AddListener(OnClicked);
        }

        private void Start() => StartCoroutine(FadeOutRoutine());

        private void OnDestroy() => _goNextButton.onClick.RemoveListener(OnClicked);

        private void OnClicked() => ButtonClicked?.Invoke(GameLoopButtonAction.LoadMenu, this);

        private IEnumerator FadeOutRoutine()
        {
            while (_canvasGroup.alpha <= 1)
            {
                _canvasGroup.alpha += 0.03f;
                yield return new WaitForSeconds(0.03f);
            }
        }
    }
}