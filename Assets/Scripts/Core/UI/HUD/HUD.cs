using System;
using System.Collections.Generic;
using System.Linq;
using Core.Database;
using Core.Logic;
using Core.Services;
using Core.Unit.Ability;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.UI.HUD
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private TurnLine _turnLine;
        [SerializeField] private AbilitiesHUD _abilities;
        [SerializeField] private Image _currentUnitIndicator;
        [SerializeField] private Sprite _currentUnitIndicatorDefaultSprite;
        [SerializeField] private Button _skipTurn;
        [SerializeField] private RoundIndicator _roundIndicator;
        [SerializeField] private OpenWindowButton _systemMenuButton;
        [SerializeField] private List<ButtonBehaviour> _buttonBehaviours;

        private ITurnResolver _turnResolver;
        private ISoundService _sound;
        private EventReference _buttonHoverSfx;
        private EventReference _buttonClickSfx;

        public event Action<BaseAbility> AbilitySelected;
        public event Action SkipTurn;
        public event Action AbilityDeselected;

        [Inject]
        public void Construct(ITurnResolver turnResolver, IWindowService windowService, ISoundService sound, IDatabaseService data)
        {
            _turnResolver = turnResolver;

            _sound = sound;
            _turnResolver.TurnStarted += OnNewTurn;
            _turnLine.Construct(_turnResolver);
            _roundIndicator.Construct(_turnResolver);
            _systemMenuButton.Init(windowService);
            
            MusicSettings musicData = data.TryGetMusicData();

            _buttonHoverSfx = musicData.AbilityButtonHovered;
            _buttonClickSfx = musicData.AbilityButtonHovered;

            // foreach (ButtonBehaviour behaviour in _buttonBehaviours)
            // {
            //     behaviour.Clicked += OnButtonClicked;
            //     behaviour.Hovered += OnButtonHovered;
            // }
        }


        private void OnDestroy()
        {
            _turnResolver.TurnStarted -= OnNewTurn;
            _abilities.AbilitySelected -= OnAbilitySelected;
            _abilities.AbilityDeselected -= OnAbilityDeselected;
        }

        private void OnNewTurn()
        {
            _skipTurn.onClick.RemoveAllListeners();
            if (_turnResolver.CurrentPlayer.IsLocal)
            {
                _abilities.ShowAbilities(_turnResolver.ActiveUnit);
                _currentUnitIndicator.sprite = _turnResolver.ActiveUnit.Config.UiImage;
                _abilities.AbilitySelected += OnAbilitySelected;
                _abilities.AbilityDeselected += OnAbilityDeselected;
                //_abilities.SkipTurn += OnTurnSkip;
                _skipTurn.onClick.AddListener(OnTurnSkip);
                Debug.Log("SKIP Add");

            }
            else
            {
                _currentUnitIndicator.sprite = _currentUnitIndicatorDefaultSprite;
                _abilities.HideAbilities();
                _abilities.AbilitySelected -= OnAbilitySelected;
                _abilities.AbilityDeselected -= OnAbilityDeselected;
                //_abilities.SkipTurn -= OnTurnSkip;
                _skipTurn.onClick.RemoveAllListeners();
                Debug.Log("SKIP REMOVE");
            }
        }


        private void OnAbilityDeselected() => AbilityDeselected?.Invoke();

        private void OnAbilitySelected(BaseAbility ability)
        {
            AbilitySelected?.Invoke(_turnResolver.ActiveUnit.Abilities
                .Where(unitAbility => unitAbility.Id == ability.Id)
                .Select(unitAbility => unitAbility)
                .First());
        }

        private void OnTurnSkip()
        {
            Debug.Log("SKIP HUD");
            SkipTurn?.Invoke();
            //_abilities.SkipTurn -= OnTurnSkip;
        }

        private void OnButtonHovered() => _sound.PlayOnce(_buttonHoverSfx);
        
        private void OnButtonClicked() => _sound.PlayOnce(_buttonClickSfx);
    }
}