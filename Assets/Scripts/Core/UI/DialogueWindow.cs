using Core.Infrastructure;
using Core.Services;
using Core.Services.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.UI
{
    public class DialogueWindow : BaseWindow
    {
        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _firstSpeakerPortrait;
        [SerializeField] private TMP_Text _firstSpeakerLabel;
        [SerializeField] private Image _secondSpeakerPortrait;
        [SerializeField] private TMP_Text _secondSpeakerLabel;
        [SerializeField] private Animator _animator;

        private HUD.HUD _hud;
        private IInputService _input;
        private DialoguePlayer _dialoguePlayer;

        public TMP_Text Text
        {
            get => _text;
            set => _text = value;
        }
        public Image FirstSpeaker => _firstSpeakerPortrait;
        public TMP_Text FirstSpeakerLabel => _firstSpeakerLabel;
        public Image SecondSpeaker => _secondSpeakerPortrait;
        public TMP_Text SecondSpeakerLabel => _secondSpeakerLabel;
        public Animator PortaitAnimator => _animator;

        [Inject]
        public void Construct(IGameFactory gameFactory, IInputService input, DialoguePlayer dialoguePlayer)
        {
            _hud = gameFactory.UI.HUD;
            _input = input;
            _hud.gameObject.SetActive(false);
            _input.DisableRaycaster();
            _dialoguePlayer = dialoguePlayer;
        }

        private void Update()
        {
            if (Input.anyKeyDown)
                _dialoguePlayer.PlayNext();
        }

        public void OnDestroy()
        {
            _hud.gameObject.SetActive(true);
            _input.EnableRaycaster();
        }
    }
}