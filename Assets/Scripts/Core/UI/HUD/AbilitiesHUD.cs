using System;
using System.Collections.Generic;
using Core.Unit;
using Core.Unit.Ability;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.HUD
{
    public class AbilitiesHUD : MonoBehaviour
    {
        [SerializeField] private List<AbilityButton> _abilities;
        [SerializeField] private Button _skipTurn;

        private bool _isAbilitySelected;
        private BaseAbility _selected;
        private AbilityButton _selectedButton;

        public event Action<BaseAbility> AbilitySelected;
        public event Action SkipTurn;
        public event Action AbilityDeselected;

        public void ShowAbilities(BaseUnit unit)
        {
            _skipTurn.onClick.RemoveListener(OnTurnSkip);
            HideAbilities();
            _skipTurn.gameObject.SetActive(true);
            _skipTurn.onClick.AddListener(OnTurnSkip);

            for (int i = 0; i < _abilities.Count; i++)
            {
                if (i >= _abilities.Count - 1)
                {
                    _abilities[i].Clicked += OnButtonClicked;
                    _abilities[i].State = AbilityButtonStateId.Empty;
                }
                else if(i < unit.Abilities.Count)
                {
                    if (unit.Abilities[i].Cooldown > 0)
                    {
                        _abilities[i].SetAbility(unit.Config.AvailableAbilities[i], unit.Abilities[i].Cooldown);
                        _abilities[i].Clicked += OnButtonClicked;
                        _abilities[i].State = AbilityButtonStateId.Cooldown;
                    }
                    else
                    {
                        _abilities[i].SetAbility(unit.Config.AvailableAbilities[i], 0);
                        _abilities[i].Clicked += OnButtonClicked;
                        _abilities[i].State = AbilityButtonStateId.Active;
                    }
                }
            }
        }

        public void HideAbilities()
        {
            _selected = null;
            _selectedButton = null;
            _skipTurn.onClick.RemoveListener(OnTurnSkip);
            _skipTurn.gameObject.SetActive(false);

            foreach (AbilityButton button in _abilities)
            {
                button.ResetAbility();
                button.Clicked -= OnButtonClicked;
            }
        }


        private void OnDestroy() => HideAbilities();

        private void OnButtonClicked(AbilityButton button)
        {
            if (button.State == AbilityButtonStateId.Cooldown || button.State == AbilityButtonStateId.Empty)
            {
                return;
            }
            if (_selected != button.AbilityData.Prefab)
            {
                AbilitySelected?.Invoke(button.AbilityData.Prefab);
                _isAbilitySelected = true;
                _selected = button.AbilityData.Prefab;
                button.State = AbilityButtonStateId.Selected;

                if (_selectedButton != null)
                {
                    _selectedButton.State = AbilityButtonStateId.Active;
                }
                _selectedButton = button;
                return;
            }
            if (_isAbilitySelected)
            {
                AbilityDeselected?.Invoke();
                _isAbilitySelected = false;
                _selected = null;
                button.State = AbilityButtonStateId.Active;
                _selectedButton = null;
            }
            else
            {
                AbilitySelected?.Invoke(button.AbilityData.Prefab);
                _isAbilitySelected = true;
                _selected = button.AbilityData.Prefab;
                button.State = AbilityButtonStateId.Selected;

                if (_selectedButton != null)
                {
                    _selectedButton.State = AbilityButtonStateId.Active;
                }
                _selectedButton = button;
            }
        }

        private void OnTurnSkip() => SkipTurn?.Invoke();
    }
}